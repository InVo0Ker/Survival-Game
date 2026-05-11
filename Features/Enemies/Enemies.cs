using Microsoft.Xna.Framework;
using SurvivalGame.Features.Player;
using System;
using System.Collections.Generic;

namespace SurvivalGame.Features.Enemies;

public enum EnemyState
{
    Walking,
    Attacking,
    Shooting
}

public enum EnemyCategory
{
    Melee,
    Ranged
}

public enum EnemyWeapon
{
    None,
    Pistol,
    Rifle,
    Sniper,
    Shotgun
}

public sealed class EnemyModel
{
    public EnemyModel(int typeIndex, Vector2 startPosition, float speedPixelsPerSecond, int damage, float attackRange, float attackCooldown, int maxHealth, EnemyCategory category, EnemyWeapon weapon = EnemyWeapon.None)
    {
        TypeIndex = typeIndex;
        Position = startPosition;
        SpeedPixelsPerSecond = speedPixelsPerSecond;
        Damage = damage;
        AttackRange = attackRange;
        AttackCooldown = attackCooldown;
        State = EnemyState.Walking;
        MaxHealth = maxHealth;
        Health = maxHealth;
        Category = category;
        Weapon = weapon;
    }

    public int TypeIndex { get; }
    public Vector2 Position { get; set; }
    public float SpeedPixelsPerSecond { get; }
    public float Rotation { get; set; }
    public float AnimationTimer { get; set; }
    public int CurrentFrame { get; set; }
    public EnemyState State { get; set; }
    public int Damage { get; }
    public float AttackRange { get; }
    public float AttackCooldown { get; }
    public float AttackTimer { get; set; }
    public int MaxHealth { get; }
    public int Health { get; set; }
    public bool IsDead { get; set; }
    public EnemyCategory Category { get; }
    public EnemyWeapon Weapon { get; }

    // Ranged behavior state
    public int BurstCount { get; set; }
    public float BurstTimer { get; set; }
}

public sealed class EnemySystem
{
    private const float FrameTime = 0.1f;
    private const int FrameCount = 9;
    private const float EnemyBulletSpeed = 400f;
    private const float SniperBulletSpeed = 800f;

    public void Update(EnemyModel enemy, PlayerModel player, List<BulletModel> bullets, GameTime gameTime)
    {
        if (enemy.IsDead) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float distanceToPlayer = Vector2.Distance(enemy.Position, player.Position);

        // Update attack cooldown timer
        if (enemy.AttackTimer > 0)
        {
            enemy.AttackTimer -= dt;
        }

        // Update burst timer for Rifles
        if (enemy.BurstTimer > 0)
        {
            enemy.BurstTimer -= dt;
            if (enemy.BurstTimer <= 0 && enemy.BurstCount > 0)
            {
                ShootSingle(enemy, player, bullets, EnemyBulletSpeed);
                enemy.BurstCount--;
                if (enemy.BurstCount > 0)
                {
                    enemy.BurstTimer = 0.1f; // Fast fire rate in burst
                }
            }
        }

        bool animate = true;

        if (distanceToPlayer <= enemy.AttackRange)
        {
            if (enemy.Category == EnemyCategory.Melee)
            {
                if (enemy.State != EnemyState.Attacking)
                {
                    enemy.State = EnemyState.Attacking;
                    enemy.CurrentFrame = 0;
                    enemy.AnimationTimer = 0;
                }

                // Deal damage at a specific frame of the attack animation
                if (enemy.CurrentFrame == 4 && enemy.AttackTimer <= 0 && player.Health > 0)
                {
                    player.ApplyDamage(enemy.Damage);
                    enemy.AttackTimer = enemy.AttackCooldown;
                }
            }
            else // Ranged
            {
                if (enemy.State != EnemyState.Shooting)
                {
                    enemy.State = EnemyState.Shooting;
                    enemy.CurrentFrame = 0; // Stand with no animation
                    enemy.AnimationTimer = 0;
                }

                animate = false; // Stop animation while shooting

                if (enemy.AttackTimer <= 0 && player.Health > 0)
                {
                    PerformRangedAttack(enemy, player, bullets);
                    enemy.AttackTimer = enemy.AttackCooldown;
                }

                // Face the player
                Vector2 direction = player.Position - enemy.Position;
                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                    enemy.Rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
                }
            }
        }
        else
        {
            if (enemy.State != EnemyState.Walking)
            {
                enemy.State = EnemyState.Walking;
                enemy.CurrentFrame = 0;
                enemy.AnimationTimer = 0;
            }

            // Follow player logic
            Vector2 direction = player.Position - enemy.Position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                enemy.Rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
                enemy.Position += direction * enemy.SpeedPixelsPerSecond * dt;
            }
        }

        // Animation logic
        if (animate)
        {
            enemy.AnimationTimer += dt;
            if (enemy.AnimationTimer >= FrameTime)
            {
                enemy.AnimationTimer -= FrameTime;
                enemy.CurrentFrame = (enemy.CurrentFrame + 1) % FrameCount;
            }
        }
        else
        {
            enemy.CurrentFrame = 0; // Force standing frame
        }
    }

    private void PerformRangedAttack(EnemyModel enemy, PlayerModel player, List<BulletModel> bullets)
    {
        switch (enemy.Weapon)
        {
            case EnemyWeapon.Pistol:
                ShootSingle(enemy, player, bullets, EnemyBulletSpeed);
                break;
            case EnemyWeapon.Rifle:
                // Start a burst of 3-5 bullets
                Random rnd = new();
                enemy.BurstCount = rnd.Next(3, 6);
                ShootSingle(enemy, player, bullets, EnemyBulletSpeed);
                enemy.BurstCount--;
                enemy.BurstTimer = 0.1f;
                break;
            case EnemyWeapon.Sniper:
                ShootSingle(enemy, player, bullets, SniperBulletSpeed);
                break;
            case EnemyWeapon.Shotgun:
                ShootShotgun(enemy, player, bullets);
                break;
        }
    }

    private void ShootSingle(EnemyModel enemy, PlayerModel player, List<BulletModel> bullets, float speed)
    {
        Vector2 direction = player.Position - enemy.Position;
        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            bullets.Add(new BulletModel(enemy.Position, direction * speed, enemy.Damage, isFromEnemy: true));
        }
    }

    private void ShootShotgun(EnemyModel enemy, PlayerModel player, List<BulletModel> bullets)
    {
        Vector2 directionToPlayer = player.Position - enemy.Position;
        if (directionToPlayer == Vector2.Zero) return;
        directionToPlayer.Normalize();

        float baseRotation = (float)Math.Atan2(directionToPlayer.Y, directionToPlayer.X);
        float spreadAngle = MathHelper.ToRadians(60); // Total spread
        int bulletCount = 5;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = baseRotation - (spreadAngle / 2) + (spreadAngle / (bulletCount - 1) * i);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            bullets.Add(new BulletModel(enemy.Position, direction * EnemyBulletSpeed, enemy.Damage, isFromEnemy: true));
        }
    }

    public void TakeDamage(EnemyModel enemy, int damage)
    {
        if (enemy.IsDead) return;

        enemy.Health -= damage;
        if (enemy.Health <= 0)
        {
            enemy.Health = 0;
            enemy.IsDead = true;
        }
    }
}
