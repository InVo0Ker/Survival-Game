using Microsoft.Xna.Framework;
using SurvivalGame.Features.Enemies;
using System;
using System.Collections.Generic;

namespace SurvivalGame.Features.Player;

public enum ProjectileType
{
    Standard,
    Grenade,
    Flame
}

public sealed class BulletModel
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public bool IsActive { get; set; } = true;
    public int Damage { get; set; }
    public bool IsFromEnemy { get; set; }
    public ProjectileType Type { get; set; }
    public float LifeTimer { get; set; }

    public BulletModel(Vector2 position, Vector2 velocity, int damage, bool isFromEnemy = false, ProjectileType type = ProjectileType.Standard)
    {
        Position = position;
        Velocity = velocity;
        Damage = damage;
        IsFromEnemy = isFromEnemy;
        Type = type;
        LifeTimer = type == ProjectileType.Flame ? 0.5f : 5.0f;
    }
}

public sealed class ExplosionModel
{
    public Vector2 Position { get; set; }
    public float Timer { get; set; }
    public int CurrentFrame { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class BulletSystem
{
    private const float BulletSpeed = 800f;
    private const float GrenadeSpeed = 600f;
    private const float FlameSpeed = 400f;
    private const float HitRadius = 15f;
    private const float ExplosionRadius = 200f;

    public List<ExplosionModel> Explosions { get; } = new();

    public void Update(List<BulletModel> bullets, List<EnemyModel> enemies, EnemySystem enemySystem, PlayerModel player, GameTime gameTime, Rectangle viewportBounds)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Update Explosions
        for (int i = Explosions.Count - 1; i >= 0; i--)
        {
            var ex = Explosions[i];
            ex.Timer += dt;
            if (ex.Timer >= 0.08f)
            {
                ex.Timer -= 0.08f;
                ex.CurrentFrame++;
                if (ex.CurrentFrame >= 4) ex.IsActive = false;
            }
            if (!ex.IsActive) Explosions.RemoveAt(i);
        }

        // Update Bullets
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            var bullet = bullets[i];
            bullet.Position += bullet.Velocity * dt;
            bullet.LifeTimer -= dt;

            if (bullet.LifeTimer <= 0) bullet.IsActive = false;

            if (bullet.IsActive)
            {
                if (bullet.IsFromEnemy)
                {
                    if (Vector2.Distance(bullet.Position, player.Position) < HitRadius)
                    {
                        player.ApplyDamage(bullet.Damage);
                        bullet.IsActive = false;
                    }
                }
                else
                {
                    foreach (var enemy in enemies)
                    {
                        if (enemy.IsDead) continue;

                        if (Vector2.Distance(bullet.Position, enemy.Position) < HitRadius)
                        {
                            if (bullet.Type == ProjectileType.Grenade)
                            {
                                CreateExplosion(bullet.Position, enemies, enemySystem);
                            }
                            else
                            {
                                enemySystem.TakeDamage(enemy, bullet.Damage);
                            }
                            bullet.IsActive = false;
                            break;
                        }
                    }
                }
            }

            if (!bullet.IsActive || !viewportBounds.Contains(bullet.Position))
            {
                bullets.RemoveAt(i);
            }
        }
    }

    private void CreateExplosion(Vector2 position, List<EnemyModel> enemies, EnemySystem enemySystem)
    {
        Explosions.Add(new ExplosionModel { Position = position });
        foreach (var enemy in enemies)
        {
            if (enemy.IsDead) continue;
            if (Vector2.Distance(position, enemy.Position) < ExplosionRadius)
            {
                enemySystem.TakeDamage(enemy, 300);
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
        // Add PiOver2 to compensate for the visual offset (-PiOver2) applied to the player sprite
        float fireRotation = player.Rotation + MathHelper.PiOver2;
        Vector2 dir = new((float)Math.Cos(fireRotation), (float)Math.Sin(fireRotation));
        Vector2 pos = player.Position + dir * 30f; // Offset from player center

        switch (player.CurrentWeapon.Type)
        {
            case WeaponType.GrenadeLauncher:
                bullets.Add(new BulletModel(pos, dir * GrenadeSpeed, player.CurrentWeapon.BulletDamage, false, ProjectileType.Grenade));
                break;
            case WeaponType.Flamethrower:
                // Spawn multiple flame particles with slight spread
                Random rnd = new();
                for (int i = 0; i < 3; i++)
                {
                    float angle = fireRotation + (float)(rnd.NextDouble() * 0.4 - 0.2);
                    Vector2 fDir = new((float)Math.Cos(angle), (float)Math.Sin(angle));
                    bullets.Add(new BulletModel(pos, fDir * FlameSpeed, player.CurrentWeapon.BulletDamage, false, ProjectileType.Flame));
                }
                break;
            default:
                bullets.Add(new BulletModel(pos, dir * BulletSpeed, player.CurrentWeapon.BulletDamage));
                break;
        }
    }
}

