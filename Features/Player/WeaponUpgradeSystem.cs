using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SurvivalGame.Features.Player;

public sealed class UpgradeBoxModel
{
    public Vector2 Position { get; set; }
    public bool IsActive { get; set; }
    public float SpawnTimer { get; set; }
    public WeaponType? DroppedWeapon { get; set; }
    public bool WaitingForDecision { get; set; }
}

public sealed class WeaponUpgradeSystem
{
    private const float SpawnInterval = 30.0f;
    private const float CollectionRadius = 40f;
    private readonly Random _random = new();

    public void Update(UpgradeBoxModel box, PlayerModel player, Rectangle viewportBounds, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (player.State == PlayerState.Dead) return;

        if (!box.IsActive)
        {
            box.SpawnTimer += dt;
            if (box.SpawnTimer >= SpawnInterval)
            {
                SpawnBox(box, viewportBounds);
            }
        }
        else if (box.WaitingForDecision)
        {
            HandleWeaponDecision(box, player);
        }
        else
        {
            if (Vector2.Distance(player.Position, box.Position) < CollectionRadius)
            {
                RollForWeapon(box);
                ProcessWeaponPickup(box, player);
            }
        }
    }

    private void SpawnBox(UpgradeBoxModel box, Rectangle viewportBounds)
    {
        int margin = 100;
        box.Position = new Vector2(
            _random.Next(viewportBounds.Left + margin, viewportBounds.Right - margin),
            _random.Next(viewportBounds.Top + margin, viewportBounds.Bottom - margin)
        );
        box.IsActive = true;
        box.DroppedWeapon = null;
        box.WaitingForDecision = false;
    }

    private void RollForWeapon(UpgradeBoxModel box)
    {
        // Randomly pick a weapon (excluding Pistol as it's default)
        Array weapons = Enum.GetValues(typeof(WeaponType));
        box.DroppedWeapon = (WeaponType)weapons.GetValue(_random.Next(1, weapons.Length));
    }

    private void ProcessWeaponPickup(UpgradeBoxModel box, PlayerModel player)
    {
        if (box.DroppedWeapon == null) return;

        // If player already has this weapon type, just refill ammo
        foreach (var w in player.Inventory)
        {
            if (w.Type == box.DroppedWeapon.Value)
            {
                w.CurrentAmmo = w.MaxAmmo;
                box.IsActive = false;
                box.SpawnTimer = 0f;
                return;
            }
        }

        // If inventory has space, add it
        if (player.Inventory.Count < 2)
        {
            player.Inventory.Add(CreateWeapon(box.DroppedWeapon.Value));
            player.CurrentWeaponIndex = player.Inventory.Count - 1;
            box.IsActive = false;
            box.SpawnTimer = 0f;
        }
        else
        {
            // Inventory full, wait for decision (Replace 1 or 2, or Reject)
            box.WaitingForDecision = true;
        }
    }

    private void HandleWeaponDecision(UpgradeBoxModel box, PlayerModel player)
    {
        KeyboardState kb = Keyboard.GetState();
        if (kb.IsKeyDown(Keys.D1)) // Replace weapon 1
        {
            player.Inventory[0] = CreateWeapon(box.DroppedWeapon.Value);
            player.CurrentWeaponIndex = 0;
            FinalizePickup(box);
        }
        else if (kb.IsKeyDown(Keys.D2)) // Replace weapon 2
        {
            player.Inventory[1] = CreateWeapon(box.DroppedWeapon.Value);
            player.CurrentWeaponIndex = 1;
            FinalizePickup(box);
        }
        else if (kb.IsKeyDown(Keys.N)) // Reject
        {
            FinalizePickup(box);
        }
    }

    private void FinalizePickup(UpgradeBoxModel box)
    {
        box.IsActive = false;
        box.WaitingForDecision = false;
        box.SpawnTimer = 0f;
    }

    private PlayerWeapon CreateWeapon(WeaponType type)
    {
        return type switch
        {
            WeaponType.MachineGun => new PlayerWeapon(type, 20, 0.1f, true, 50),
            WeaponType.Rifle => new PlayerWeapon(type, 35, 0.2f, true, 30),
            WeaponType.GrenadeLauncher => new PlayerWeapon(type, 300, 1.5f, false, 1, 4.0f), // RPG
            WeaponType.Flamethrower => new PlayerWeapon(type, 10, 0.05f, true, 100),
            _ => new PlayerWeapon(WeaponType.Pistol, 25, 0.25f, false, 30)
        };
    }

    public void Reset(UpgradeBoxModel box)
    {
        box.IsActive = false;
        box.SpawnTimer = 0f;
        box.WaitingForDecision = false;
    }
}
