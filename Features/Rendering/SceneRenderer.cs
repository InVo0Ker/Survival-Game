using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurvivalGame.Features.Player;
using SurvivalGame.Features.Enemies;

namespace SurvivalGame.Features.Rendering;

public sealed class SceneRenderer
{
    private Texture2D _pixel;

    private void EnsurePixel(GraphicsDevice graphicsDevice)
    {
        if (_pixel == null)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }
    }

    public void Draw(
        SpriteBatch spriteBatch,
        Texture2D backgroundTexture,
        Texture2D playerTexture,
        PlayerModel player,
        System.Collections.Generic.List<EnemyModel> enemies,
        Texture2D[] enemyWalkTextures,
        Texture2D[] enemyAttackTextures,
        Texture2D crosshairTexture,
        System.Collections.Generic.List<BulletModel> bullets,
        UpgradeBoxModel upgradeBox,
        Texture2D boxTexture,
        Rectangle viewportBounds)
    {
        EnsurePixel(spriteBatch.GraphicsDevice);
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        spriteBatch.Draw(backgroundTexture, destinationRectangle: viewportBounds, Color.White);

        // Draw Upgrade Box
        if (upgradeBox.IsActive)
        {
            Vector2 boxOrigin = new Vector2(boxTexture.Width * 0.5f, boxTexture.Height * 0.5f);
            spriteBatch.Draw(boxTexture, upgradeBox.Position, null, Color.White, 0f, boxOrigin, 1f, SpriteEffects.None, 0f);
        }

        // Draw Bullets
        foreach (var bullet in bullets)
        {
            spriteBatch.Draw(_pixel, new Rectangle((int)bullet.Position.X - 2, (int)bullet.Position.Y - 2, 4, 4), Color.Yellow);
        }

        // Draw Enemies
        foreach (var enemy in enemies)
        {
            if (enemy.IsDead) continue;

            Texture2D[] currentAnimation = enemy.State == EnemyState.Attacking ? enemyAttackTextures : enemyWalkTextures;
            Texture2D currentEnemyTexture = currentAnimation[enemy.CurrentFrame];
            Vector2 enemyOrigin = new(currentEnemyTexture.Width * 0.5f, currentEnemyTexture.Height * 0.5f);
            spriteBatch.Draw(
                currentEnemyTexture,
                position: enemy.Position,
                sourceRectangle: null,
                color: Color.White,
                rotation: enemy.Rotation,
                origin: enemyOrigin,
                scale: 0.1f,
                effects: SpriteEffects.None,
                layerDepth: 0f);
        }

        // Draw Player
        if (player.Health > 0)
        {
            Vector2 playerOrigin = new(playerTexture.Width * 0.5f, playerTexture.Height * 0.5f);
            spriteBatch.Draw(
                playerTexture,
                position: player.Position,
                sourceRectangle: null,
                color: Color.White,
                rotation: player.Rotation,
                origin: playerOrigin,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 0f);
        }

        // Draw HUD
        DrawHealthBar(spriteBatch, player);
        DrawAmmoInfo(spriteBatch, player);

        // Draw Game Over Screen
        if (player.Health <= 0)
        {
            // Dark overlay
            spriteBatch.Draw(_pixel, viewportBounds, Color.Black * 0.5f);

            // "Game Over" Red Bar
            int barWidth = 400;
            int barHeight = 60;
            Rectangle gameOverRect = new Rectangle(
                viewportBounds.Center.X - barWidth / 2,
                viewportBounds.Center.Y - barHeight / 2,
                barWidth, barHeight);
            spriteBatch.Draw(_pixel, gameOverRect, Color.DarkRed);
            
            // "Press ENTER to Restart" indicator
            spriteBatch.Draw(_pixel, new Rectangle(gameOverRect.X, gameOverRect.Bottom + 10, barWidth, 20), Color.Black);
            spriteBatch.Draw(_pixel, new Rectangle(gameOverRect.X, gameOverRect.Bottom + 10, (int)(barWidth * 0.5f), 20), Color.Cyan * 0.5f);
        }

        // Draw Crosshair
        MouseState mouse = Mouse.GetState();
        Vector2 crosshairPosition = new Vector2(mouse.X, mouse.Y);
        Vector2 crosshairOrigin = new Vector2(crosshairTexture.Width * 0.5f, crosshairTexture.Height * 0.5f);
        spriteBatch.Draw(crosshairTexture, crosshairPosition, null, Color.White, 0f, crosshairOrigin, 1f, SpriteEffects.None, 0f);

        spriteBatch.End();
    }

    private void DrawAmmoInfo(SpriteBatch spriteBatch, PlayerModel player)
    {
        int barWidth = 100;
        int barHeight = 5;
        Vector2 barPosition = new Vector2(20, 35);

        // Ammo Bar Background
        spriteBatch.Draw(_pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Gray);

        // Ammo Bar Foreground
        float ammoPercent = (float)player.CurrentAmmo / player.MaxAmmo;
        spriteBatch.Draw(_pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * ammoPercent), barHeight), Color.Yellow);

        if (player.IsReloading)
        {
            // Simple indicator for reloading
            spriteBatch.Draw(_pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y + 10, (int)(barWidth * (1 - player.ReloadTimer / 2.0f)), 5), Color.Cyan);
        }
    }

    private void DrawHealthBar(SpriteBatch spriteBatch, PlayerModel player)
    {
        int barWidth = 100;
        int barHeight = 10;
        Vector2 barPosition = new Vector2(20, 20);

        // Background (Red)
        spriteBatch.Draw(_pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Red);

        // Foreground (Green)
        int currentWidth = (int)(barWidth * ((float)player.Health / player.MaxHealth));
        spriteBatch.Draw(_pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, currentWidth, barHeight), Color.Green);
    }
}
