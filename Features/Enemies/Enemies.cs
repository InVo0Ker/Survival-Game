using Microsoft.Xna.Framework;
using SurvivalGame.Features.Player;
using System;

namespace SurvivalGame.Features.Enemies;

public sealed class EnemyModel
{
    public EnemyModel(Vector2 startPosition, float speedPixelsPerSecond)
    {
        Position = startPosition;
        SpeedPixelsPerSecond = speedPixelsPerSecond;
    }

    public Vector2 Position { get; set; }
    public float SpeedPixelsPerSecond { get; }
    public float Rotation { get; set; }
    public float AnimationTimer { get; set; }
    public int CurrentFrame { get; set; }
}

public sealed class EnemySystem
{
    private const float FrameTime = 0.1f;
    private const int FrameCount = 9;

    public void Update(EnemyModel enemy, PlayerModel player, GameTime gameTime)
    {
        // Follow player logic
        Vector2 direction = player.Position - enemy.Position;
        
        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            // Added PiOver2 offset assuming sprite faces Up by default
            enemy.Rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        enemy.Position += direction * enemy.SpeedPixelsPerSecond * dt;

        // Animation logic
        enemy.AnimationTimer += dt;
        if (enemy.AnimationTimer >= FrameTime)
        {
            enemy.AnimationTimer -= FrameTime;
            enemy.CurrentFrame = (enemy.CurrentFrame + 1) % FrameCount;
        }
    }
}
