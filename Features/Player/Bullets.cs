using Microsoft.Xna.Framework;
using SurvivalGame.Features.Enemies;
using System.Collections.Generic;

namespace SurvivalGame.Features.Player;

public sealed class BulletModel
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public bool IsActive { get; set; } = true;
    public int Damage { get; set; }
    public bool IsFromEnemy { get; set; }

    public BulletModel(Vector2 position, Vector2 velocity, int damage, bool isFromEnemy = false)
    {
        Position = position;
        Velocity = velocity;
        Damage = damage;
        IsFromEnemy = isFromEnemy;
    }
}

public sealed class BulletSystem
{
    private const float BulletSpeed = 800f;
    private const float HitRadius = 15f;

    public void Update(List<BulletModel> bullets, List<EnemyModel> enemies, EnemySystem enemySystem, PlayerModel player, GameTime gameTime, Rectangle viewportBounds)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            var bullet = bullets[i];
            bullet.Position += bullet.Velocity * dt;

            if (bullet.IsFromEnemy)
            {
                // Check collision with player
                if (Vector2.Distance(bullet.Position, player.Position) < HitRadius)
                {
                    player.ApplyDamage(bullet.Damage);
                    bullet.IsActive = false;
                }
            }
            else
            {
                // Check collision with enemies
                foreach (var enemy in enemies)
                {
                    if (enemy.IsDead) continue;

                    if (Vector2.Distance(bullet.Position, enemy.Position) < HitRadius)
                    {
                        enemySystem.TakeDamage(enemy, bullet.Damage);
                        bullet.IsActive = false;
                        break;
                    }
                }
            }

            if (!bullet.IsActive || !viewportBounds.Contains(bullet.Position))
            {
                bullets.RemoveAt(i);
            }
        }
    }

    public void UpdateTimers(PlayerModel player, GameTime gameTime)
    {
        if (player.ShootTimer > 0)
        {
            player.ShootTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    public void Shoot(List<BulletModel> bullets, PlayerModel player)
    {
        if (player.CurrentAmmo > 0 && !player.IsReloading && player.ShootTimer <= 0)
        {
            Vector2 direction = new Vector2((float)System.Math.Cos(player.Rotation), (float)System.Math.Sin(player.Rotation));
            bullets.Add(new BulletModel(player.Position, direction * BulletSpeed, player.BulletDamage, isFromEnemy: false));
            player.CurrentAmmo--;
            player.ShootTimer = player.FireRate;
        }
    }
}
