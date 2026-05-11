using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace SurvivalGame.Features.Player;

public sealed class PlayerMovementSystem
{
    private const float WalkFrameTime = 0.1f;
    private const int WalkFrameCount = 7;
    private const float DeathFrameTime = 0.2f;
    private const int DeathFrameCount = 4;

    public void Update(PlayerModel player, GameTime gameTime, Rectangle worldBounds)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        player.UpdateTimedEffects(dt);

        if (player.State == PlayerState.Dead)
        {
            UpdateDeathAnimation(player, dt);
            return;
        }

        // Handle Weapon Switching
        KeyboardState keyboard = Keyboard.GetState();
        if (keyboard.IsKeyDown(Keys.D1)) player.CurrentWeaponIndex = 0;
        if (keyboard.IsKeyDown(Keys.D2) && player.Inventory.Count > 1) player.CurrentWeaponIndex = 1;

        // Handle Movement
        Vector2 direction = Vector2.Zero;
        if (keyboard.IsKeyDown(Keys.W)) direction.Y -= 1f;
        if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1f;
        if (keyboard.IsKeyDown(Keys.A)) direction.X -= 1f;
        if (keyboard.IsKeyDown(Keys.D)) direction.X += 1f;

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            player.State = PlayerState.Walking;
            player.MoveRotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
            UpdateWalkAnimation(player, dt);
        }
        else
        {
            player.State = PlayerState.Idle;
            player.CurrentFrame = 0;
            player.AnimationTimer = 0f;
        }

        player.Position += direction * player.SpeedPixelsPerSecond * dt;
        player.Position = Vector2.Clamp(
            player.Position,
            new Vector2(worldBounds.Left, worldBounds.Top),
            new Vector2(worldBounds.Right, worldBounds.Bottom));

        // Handle Rotation towards mouse
        MouseState mouse = Mouse.GetState();
        Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);
        Vector2 lookDirection = mousePosition - player.Position;
        if (lookDirection != Vector2.Zero)
        {
            player.Rotation = (float)Math.Atan2(lookDirection.Y, lookDirection.X) - MathHelper.PiOver2;
        }

        // Handle Reloading
        if (player.IsReloading)
        {
            player.ReloadTimer -= dt;
            if (player.ReloadTimer <= 0)
            {
                player.CurrentWeapon.CurrentAmmo = player.CurrentWeapon.MaxAmmo;
                player.IsReloading = false;
            }
        }
        else if (keyboard.IsKeyDown(Keys.R) && player.CurrentWeapon.CurrentAmmo < player.CurrentWeapon.MaxAmmo)
        {
            StartReload(player);
        }
    }

    private void UpdateWalkAnimation(PlayerModel player, float dt)
    {
        player.AnimationTimer += dt;
        if (player.AnimationTimer >= WalkFrameTime)
        {
            player.AnimationTimer -= WalkFrameTime;
            player.CurrentFrame = (player.CurrentFrame + 1) % WalkFrameCount;
        }
    }

    private void UpdateDeathAnimation(PlayerModel player, float dt)
    {
        if (player.CurrentFrame < DeathFrameCount - 1)
        {
            player.DeathAnimationTimer += dt;
            if (player.DeathAnimationTimer >= DeathFrameTime)
            {
                player.DeathAnimationTimer -= DeathFrameTime;
                player.CurrentFrame++;
            }
        }
    }

    public void StartReload(PlayerModel player)
    {
        if (!player.IsReloading)
        {
            player.IsReloading = true;
            player.ReloadTimer = player.CurrentWeapon.ReloadTime;
        }
    }
}
