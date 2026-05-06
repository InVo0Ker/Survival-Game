using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace SurvivalGame.Features.Player;

public sealed class PlayerMovementSystem
{
    private const float ReloadTime = 2.0f;

    public void Update(PlayerModel player, GameTime gameTime, Rectangle worldBounds)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        player.UpdateTimedEffects(dt);

        // Handle Movement
        KeyboardState keyboard = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W)) direction.Y -= 1f;
        if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1f;
        if (keyboard.IsKeyDown(Keys.A)) direction.X -= 1f;
        if (keyboard.IsKeyDown(Keys.D)) direction.X += 1f;

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
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
            // Assuming player sprite faces Right by default, we use Atan2. 
            // If it faces Up, we might need an offset like -MathHelper.PiOver2.
            // Let's check the current zombie rotation logic: (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
            player.Rotation = (float)Math.Atan2(lookDirection.Y, lookDirection.X);
        }

        // Handle Reloading
        if (player.IsReloading)
        {
            player.ReloadTimer -= dt;
            if (player.ReloadTimer <= 0)
            {
                player.CurrentAmmo = player.MaxAmmo;
                player.IsReloading = false;
            }
        }
        else if (keyboard.IsKeyDown(Keys.R) && player.CurrentAmmo < player.MaxAmmo)
        {
            StartReload(player);
        }
    }

    public void StartReload(PlayerModel player)
    {
        if (!player.IsReloading)
        {
            player.IsReloading = true;
            player.ReloadTimer = ReloadTime;
        }
    }
}
