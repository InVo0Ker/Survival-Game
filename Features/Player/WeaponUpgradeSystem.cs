using Microsoft.Xna.Framework;
using System;

namespace SurvivalGame.Features.Player;

public sealed class UpgradeBoxModel
{
    public Vector2 Position { get; set; }
    public bool IsActive { get; set; }
    public float SpawnTimer { get; set; }
}

public sealed class WeaponUpgradeSystem
{
    private const float SpawnInterval = 60.0f; // 1 minute
    private const float CollectionRadius = 30f;
    private readonly Random _random = new();

    public void Update(UpgradeBoxModel box, PlayerModel player, Rectangle viewportBounds, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (!box.IsActive)
        {
            box.SpawnTimer += dt;
            if (box.SpawnTimer >= SpawnInterval)
            {
                SpawnBox(box, viewportBounds);
            }
        }
        else
        {
            // Check collision with player
            if (Vector2.Distance(player.Position, box.Position) < CollectionRadius)
            {
                ApplyUpgrade(player);
                box.IsActive = false;
                box.SpawnTimer = 0f;
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
    }

    private void ApplyUpgrade(PlayerModel player)
    {
        player.BulletDamage = 50;
        player.FireRate = 0.1f;
        player.IsAutomatic = true;
        // Visual indicator: Maybe refill health too?
        player.Health = player.MaxHealth;
    }

    public void Reset(UpgradeBoxModel box)
    {
        box.IsActive = false;
        box.SpawnTimer = 0f;
    }
}
