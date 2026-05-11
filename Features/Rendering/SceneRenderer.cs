using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurvivalGame.Features.Player;
using SurvivalGame.Features.Enemies;
using System.Collections.Generic;

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
        Dictionary<WeaponType, Texture2D> heroIdleTextures,
        Dictionary<WeaponType, Texture2D> heroFireTextures,
        Texture2D[] heroWalkTextures,
        Texture2D[] heroDieTextures,
        Dictionary<WeaponType, Texture2D[]> muzzleFlashes,
        Texture2D[] explosionTextures,
        Texture2D rocketTexture,
        PlayerModel player,
        List<EnemyModel> enemies,
        Texture2D[][] allEnemyWalkTextures,
        Texture2D[][] allEnemyAttackTextures,
        Texture2D crosshairTexture,
        List<BulletModel> bullets,
        List<ExplosionModel> explosions,
        List<EnemyDropModel> enemyDrops,
        UpgradeBoxModel upgradeBox,
        Texture2D crateTexture,
        Dictionary<WeaponType, Texture2D> weaponHUDIcons,
        Texture2D healthIconTexture,
        Texture2D ammoDropTexture,
        Texture2D armorDropTexture,
        Texture2D speedDropTexture,
        Rectangle viewportBounds)
    {
        EnsurePixel(spriteBatch.GraphicsDevice);
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        spriteBatch.Draw(backgroundTexture, destinationRectangle: viewportBounds, Color.White);

        // Draw Upgrade Crate
        if (upgradeBox.IsActive)
        {
            Vector2 origin = new(crateTexture.Width * 0.5f, crateTexture.Height * 0.5f);
            spriteBatch.Draw(crateTexture, upgradeBox.Position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
        }

        // Bullets & Particles
        foreach (var bullet in bullets)
        {
            if (bullet.IsFromEnemy)
                spriteBatch.Draw(_pixel, new Rectangle((int)bullet.Position.X - 2, (int)bullet.Position.Y - 2, 4, 4), Color.OrangeRed);
            else if (bullet.Type == ProjectileType.Flame)
                spriteBatch.Draw(_pixel, new Rectangle((int)bullet.Position.X - 4, (int)bullet.Position.Y - 4, 8, 8), Color.Orange * (bullet.LifeTimer / 0.5f));
            else if (bullet.Type == ProjectileType.Grenade)
            {
                Vector2 origin = new(rocketTexture.Width * 0.5f, rocketTexture.Height * 0.5f);
                // Add PiOver2 to align rocket sprite (faces Up) with its travel direction
                float rotation = (float)System.Math.Atan2(bullet.Velocity.Y, bullet.Velocity.X) + MathHelper.PiOver2;
                spriteBatch.Draw(rocketTexture, bullet.Position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
            }
            else
                spriteBatch.Draw(_pixel, new Rectangle((int)bullet.Position.X - 2, (int)bullet.Position.Y - 2, 4, 4), Color.Yellow);
        }

        // Explosions
        foreach (var ex in explosions)
        {
            Texture2D tex = explosionTextures[ex.CurrentFrame];
            Vector2 origin = new(tex.Width * 0.5f, tex.Height * 0.5f);
            spriteBatch.Draw(tex, ex.Position, null, Color.White, 0f, origin, 2.5f, SpriteEffects.None, 0f);
        }

        // Enemy Drops
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

        // Enemies
        foreach (var enemy in enemies)
        {
            if (enemy.IsDead) continue;
            Texture2D[] walkAnims = allEnemyWalkTextures[enemy.TypeIndex];
            Texture2D[] attackAnims = allEnemyAttackTextures[enemy.TypeIndex];
            Texture2D tex;
            if (enemy.State == EnemyState.Shooting && attackAnims != null) tex = attackAnims[(int)enemy.Weapon];
            else {
                Texture2D[] anim = (enemy.State == EnemyState.Attacking && attackAnims != null) ? attackAnims : walkAnims;
                tex = anim[enemy.CurrentFrame];
            }
            Vector2 origin = new(tex.Width * 0.5f, tex.Height * 0.5f);
            spriteBatch.Draw(tex, enemy.Position, null, Color.White, enemy.Rotation, origin, 0.1f, SpriteEffects.None, 0f);
        }

        // Player
        DrawPlayer(spriteBatch, heroIdleTextures, heroFireTextures, heroWalkTextures, heroDieTextures, muzzleFlashes, player);

        // HUD
        DrawHealthHearts(spriteBatch, player, healthIconTexture, armorDropTexture);
        DrawInventoryHUD(spriteBatch, player, weaponHUDIcons);

        if (upgradeBox.IsActive && upgradeBox.WaitingForDecision && upgradeBox.DroppedWeapon.HasValue)
        {
            DrawDecisionHUD(spriteBatch, weaponHUDIcons[upgradeBox.DroppedWeapon.Value], viewportBounds);
        }

        // Crosshair
        MouseState mouse = Mouse.GetState();
        Vector2 cp = new(mouse.X, mouse.Y);
        Vector2 co = new(crosshairTexture.Width * 0.5f, crosshairTexture.Height * 0.5f);
        spriteBatch.Draw(crosshairTexture, cp, null, Color.White, 0f, co, 1f, SpriteEffects.None, 0f);

        spriteBatch.End();
    }

    private void DrawPlayer(SpriteBatch sb, Dictionary<WeaponType, Texture2D> idle, Dictionary<WeaponType, Texture2D> fire, Texture2D[] walk, Texture2D[] die, Dictionary<WeaponType, Texture2D[]> flashes, PlayerModel player)
    {
        if (player.State == PlayerState.Dead)
        {
            Texture2D dieTex = die[player.CurrentFrame];
            Vector2 dieOrigin = new(dieTex.Width * 0.5f, dieTex.Height * 0.5f);
            sb.Draw(dieTex, player.Position, null, Color.White, player.Rotation, dieOrigin, 1f, SpriteEffects.None, 0f);
            return;
        }

        // Draw Legs (Walk animation) under torso
        if (player.State == PlayerState.Walking)
        {
            Texture2D legTex = walk[player.CurrentFrame];
            Vector2 legOrigin = new(legTex.Width * 0.5f, legTex.Height * 0.5f);
            sb.Draw(legTex, player.Position, null, Color.White, player.MoveRotation, legOrigin, 1f, SpriteEffects.None, 0f);
        }
        else
        {
            // If idle, we could draw frame 0 of walk as static legs
            Texture2D legTex = walk[0];
            Vector2 legOrigin = new(legTex.Width * 0.5f, legTex.Height * 0.5f);
            sb.Draw(legTex, player.Position, null, Color.White * 0.8f, player.Rotation, legOrigin, 1f, SpriteEffects.None, 0f);
        }

        // Draw Torso (Weapon)
        Texture2D torsoTex = player.MuzzleFlashTimer > 0 ? fire[player.CurrentWeapon.Type] : idle[player.CurrentWeapon.Type];
        Vector2 torsoOrigin = new(torsoTex.Width * 0.5f, torsoTex.Height * 0.5f);
        sb.Draw(torsoTex, player.Position, null, Color.White, player.Rotation, torsoOrigin, 1f, SpriteEffects.None, 0f);

        // Draw Muzzle Flash
        if (player.MuzzleFlashTimer > 0)
        {
            Texture2D[] flashAnim = flashes[player.CurrentWeapon.Type];
            int f = (int)((0.05f - player.MuzzleFlashTimer) / 0.016f) % flashAnim.Length;
            Texture2D flashTex = flashAnim[f];
            Vector2 flashOrigin = new(flashTex.Width * 0.5f, flashTex.Height * 0.5f);
            sb.Draw(flashTex, player.Position, null, Color.White, player.Rotation, flashOrigin, 1f, SpriteEffects.None, 0f);
        }
    }

    private void DrawInventoryHUD(SpriteBatch sb, PlayerModel player, Dictionary<WeaponType, Texture2D> icons)
    {
        int startX = 20; int startY = 80; int spacing = 70;
        for (int i = 0; i < player.Inventory.Count; i++)
        {
            var weapon = player.Inventory[i];
            Texture2D icon = icons[weapon.Type];
            Color tint = (i == player.CurrentWeaponIndex) ? Color.White : Color.White * 0.4f;
            sb.Draw(icon, new Vector2(startX + i * spacing, startY), tint);

            // Ammo bar for each weapon
            int barW = 60; int barH = 4;
            sb.Draw(_pixel, new Rectangle(startX + i * spacing, startY + 50, barW, barH), Color.Gray);
            float p = (float)weapon.CurrentAmmo / weapon.MaxAmmo;
            sb.Draw(_pixel, new Rectangle(startX + i * spacing, startY + 50, (int)(barW * p), barH), Color.Yellow);
        }
    }

    private void DrawDecisionHUD(SpriteBatch sb, Texture2D newWeaponIcon, Rectangle viewport)
    {
        sb.Draw(_pixel, viewport, Color.Black * 0.5f);
        Vector2 center = new(viewport.Center.X, viewport.Center.Y);
        sb.Draw(newWeaponIcon, center - new Vector2(newWeaponIcon.Width * 0.5f, 100), Color.White);
        sb.Draw(_pixel, new Rectangle((int)center.X - 150, (int)center.Y + 20, 300, 40), Color.DarkSlateGray);
    }

    private void DrawHealthHearts(SpriteBatch sb, PlayerModel player, Texture2D hp, Texture2D shield)
    {
        const int startX = 20; const int startY = 16; const int spacing = 6;
        for (int i = 0; i < player.MaxHealth; i++) {
            Color tint = i < player.Health ? Color.White : Color.White * 0.2f;
            sb.Draw(hp, new Vector2(startX + i * (hp.Width + spacing), startY), tint);
        }
        int sx = startX + player.MaxHealth * (hp.Width + spacing) + 10;
        for (int i = 0; i < player.MaxShields; i++) {
            Color tint = i < player.ShieldCount ? Color.White : Color.White * 0.2f;
            sb.Draw(shield, new Vector2(sx + i * (hp.Width + spacing), startY), tint);
        }
    }
}
