using Microsoft.Xna.Framework;
using SurvivalGame.Features.Player;
using System;

namespace SurvivalGame.Features.Enemies;

public enum EnemyState
{
    Walking,
    Attacking
}

public sealed class EnemyModel
{
    public EnemyModel(Vector2 startPosition, float speedPixelsPerSecond, int damage, float attackRange, float attackCooldown, int maxHealth)
    {
        Position = startPosition;
        SpeedPixelsPerSecond = speedPixelsPerSecond;
        Damage = damage;
        AttackRange = attackRange;
        AttackCooldown = attackCooldown;
        State = EnemyState.Walking;
        MaxHealth = maxHealth;
        Health = maxHealth;
    }

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
}

public sealed class EnemySystem
{
    private const float FrameTime = 0.1f;
    private const int FrameCount = 9;

    public void Update(EnemyModel enemy, PlayerModel player, GameTime gameTime)
    {
        if (enemy.IsDead) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float distanceToPlayer = Vector2.Distance(enemy.Position, player.Position);

        // Update attack cooldown timer
        if (enemy.AttackTimer > 0)
        {
            enemy.AttackTimer -= dt;
        }

        if (distanceToPlayer <= enemy.AttackRange)
        {
            if (enemy.State != EnemyState.Attacking)
            {
                enemy.State = EnemyState.Attacking;
                enemy.CurrentFrame = 0;
                enemy.AnimationTimer = 0;
            }

            // Deal damage at a specific frame of the attack animation (e.g., middle frame)
            if (enemy.CurrentFrame == 4 && enemy.AttackTimer <= 0 && player.Health > 0)
            {
                player.ApplyDamage(enemy.Damage);
                enemy.AttackTimer = enemy.AttackCooldown;
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
        enemy.AnimationTimer += dt;
        if (enemy.AnimationTimer >= FrameTime)
        {
            enemy.AnimationTimer -= FrameTime;
            enemy.CurrentFrame = (enemy.CurrentFrame + 1) % FrameCount;
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
