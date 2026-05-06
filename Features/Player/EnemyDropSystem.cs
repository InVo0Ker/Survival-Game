using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SurvivalGame.Features.Player;

public enum EnemyDropType
{
    Health,
    Ammo,
    Armor,
    Speed
}

public sealed class EnemyDropModel
{
    public EnemyDropModel(Vector2 position, EnemyDropType type)
    {
        Position = position;
        Type = type;
        Lifetime = 0f;
    }

    public Vector2 Position { get; }
    public EnemyDropType Type { get; }
    public float Lifetime { get; set; }

    public float Opacity
    {
        get
        {
            if (Lifetime <= 3f) return 1f;
            
            float fadeProgress = (Lifetime - 3f) / 2f;
            return MathHelper.Clamp(1f - fadeProgress, 0f, 1f);
        }
    }
}

public sealed class EnemyDropSystem
{
    private const double DropChancePerType = 0.05;
    private const float PickupRadius = 24f;
    private const float SpeedBoostMultiplier = 1.2f;
    private const float SpeedBoostDurationSeconds = 7f;
    private readonly Random _random = new();

    public void TrySpawnOnEnemyDeath(List<EnemyDropModel> drops, Vector2 enemyPosition)
    {
        TrySpawn(drops, enemyPosition, EnemyDropType.Health);
        TrySpawn(drops, enemyPosition, EnemyDropType.Ammo);
        TrySpawn(drops, enemyPosition, EnemyDropType.Armor);
        TrySpawn(drops, enemyPosition, EnemyDropType.Speed);
    }

    public void UpdateCollection(List<EnemyDropModel> drops, PlayerModel player, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int i = drops.Count - 1; i >= 0; i--)
        {
            EnemyDropModel drop = drops[i];
            
            drop.Lifetime += deltaTime;

            if (drop.Lifetime >= 5f)
            {
                drops.RemoveAt(i);
                continue;
            }

            if (Vector2.Distance(player.Position, drop.Position) <= PickupRadius)
            {
                if (TryApplyDrop(player, drop.Type))
                {
                    drops.RemoveAt(i);
                }
            }
        }
    }

    private void TrySpawn(List<EnemyDropModel> drops, Vector2 enemyPosition, EnemyDropType type)
    {
        if (_random.NextDouble() <= DropChancePerType)
        {
            drops.Add(new EnemyDropModel(enemyPosition, type));
        }
    }

    private static bool TryApplyDrop(PlayerModel player, EnemyDropType type)
    {
        switch (type)
        {
            case EnemyDropType.Health:
                if (player.Health >= player.MaxHealth) return false;
                player.AddHealth(1);
                return true;
            case EnemyDropType.Ammo:
                player.InstantReload();
                return true;
            case EnemyDropType.Armor:
                if (player.ShieldCount >= player.MaxShields) return false;
                player.AddShield(1);
                return true;
            case EnemyDropType.Speed:
                player.ActivateSpeedBoost(SpeedBoostMultiplier, SpeedBoostDurationSeconds);
                return true;
            default:
                return false;
        }
    }
}