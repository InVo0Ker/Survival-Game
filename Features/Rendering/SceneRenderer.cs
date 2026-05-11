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
        Texture2D[][] allEnemyWalkTextures,
        Texture2D[][] allEnemyAttackTextures,
        Texture2D crosshairTexture,
        System.Collections.Generic.List<BulletModel> bullets,
        System.Collections.Generic.List<EnemyDropModel> enemyDrops,
        UpgradeBoxModel upgradeBox,
        Texture2D boxTexture,
        Texture2D healthIconTexture,
        Texture2D ammoDropTexture,
        Texture2D armorDropTexture,
        Texture2D speedDropTexture,
        Rectangle viewportBounds)
    {
        EnsurePixel(spriteBatch.GraphicsDevice);
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        spriteBatch.Draw(backgroundTexture, destinationRectangle: viewportBounds, Color.White);

        if (upgradeBox.IsActive)
        {
            Vector2 boxOrigin = new Vector2(boxTexture.Width * 0.5f, boxTexture.Height * 0.5f);
            spriteBatch.Draw(boxTexture, upgradeBox.Position, null, Color.White, 0f, boxOrigin, 1f, SpriteEffects.None, 0f);
        }

        foreach (var bullet in bullets)
        {
            Color bulletColor = bullet.IsFromEnemy ? Color.OrangeRed : Color.Yellow;
            spriteBatch.Draw(_pixel, new Rectangle((int)bullet.Position.X - 2, (int)bullet.Position.Y - 2, 4, 4), bulletColor);
        }

        foreach (var drop in enemyDrops)
        {
            Texture2D texture = drop.Type switch
            {
                EnemyDropType.Health => healthIconTexture,
                EnemyDropType.Ammo => ammoDropTexture,
                EnemyDropType.Armor => armorDropTexture,
                EnemyDropType.Speed => speedDropTexture,
                _ => healthIconTexture
            };

            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);

            spriteBatch.Draw(texture, drop.Position, null, Color.White * drop.Opacity, 0f, origin, 1f, SpriteEffects.None, 0f);
        }

        foreach (var enemy in enemies)
        {
            if (enemy.IsDead) continue;

            Texture2D[] walkAnims = allEnemyWalkTextures[enemy.TypeIndex];
            Texture2D[] attackAnims = allEnemyAttackTextures[enemy.TypeIndex];

            Texture2D currentEnemyTexture;
            if (enemy.State == EnemyState.Shooting && attackAnims != null)
            {
                // For Ranged zombies, attackAnims contains weapon-specific textures
                currentEnemyTexture = attackAnims[(int)enemy.Weapon];
            }
            else
            {
                Texture2D[] currentAnimation = (enemy.State == EnemyState.Attacking && attackAnims != null) ? attackAnims : walkAnims;
                currentEnemyTexture = currentAnimation[enemy.CurrentFrame];
            }

            Vector2 enemyOrigin = new(currentEnemyTexture.Width * 0.5f, currentEnemyTexture.Height * 0.5f);
            spriteBatch.Draw(currentEnemyTexture, enemy.Position, null, Color.White, enemy.Rotation, enemyOrigin, 0.1f, SpriteEffects.None, 0f);
        }

        if (player.Health > 0)
        {
            Vector2 playerOrigin = new(playerTexture.Width * 0.5f, playerTexture.Height * 0.5f);
            spriteBatch.Draw(playerTexture, player.Position, null, Color.White, player.Rotation, playerOrigin, 1f, SpriteEffects.None, 0f);
        }

        DrawHealthHearts(spriteBatch, player, healthIconTexture, armorDropTexture);
        DrawAmmoInfo(spriteBatch, player);

        MouseState mouse = Mouse.GetState();
        Vector2 crosshairPosition = new Vector2(mouse.X, mouse.Y);
        Vector2 crosshairOrigin = new Vector2(crosshairTexture.Width * 0.5f, crosshairTexture.Height * 0.5f);
        spriteBatch.Draw(crosshairTexture, crosshairPosition, null, Color.White, 0f, crosshairOrigin, 1f, SpriteEffects.None, 0f);

        spriteBatch.End();
    }

    private void DrawAmmoInfo(SpriteBatch spriteBatch, PlayerModel player)
    {
        int barWidth = 100; int barHeight = 5; Vector2 barPosition = new Vector2(20, 50);
        spriteBatch.Draw(_pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Gray);
        float ammoPercent = (float)player.CurrentAmmo / player.MaxAmmo;
        spriteBatch.Draw(_pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * ammoPercent), barHeight), Color.Yellow);
        if (player.IsReloading)
            spriteBatch.Draw(_pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y + 10, (int)(barWidth * (1 - player.ReloadTimer / 2.0f)), 5), Color.Cyan);
    }

    private void DrawHealthHearts(SpriteBatch spriteBatch, PlayerModel player, Texture2D healthIconTexture, Texture2D shieldIconTexture)
    {
        const int startX = 20; const int startY = 16; const int spacing = 6;
        for (int i = 0; i < player.MaxHealth; i++) {
            Color tint = i < player.Health ? Color.White : Color.White * 0.2f;
            spriteBatch.Draw(healthIconTexture, new Vector2(startX + i * (healthIconTexture.Width + spacing), startY), tint);
        }
        int shieldsStartX = startX + player.MaxHealth * (healthIconTexture.Width + spacing) + 10;
        for (int i = 0; i < player.MaxShields; i++) {
            Color tint = i < player.ShieldCount ? Color.White : Color.White * 0.2f;
            spriteBatch.Draw(shieldIconTexture, new Vector2(shieldsStartX + i * (shieldIconTexture.Width + spacing), startY), tint);
        }
    }
}