using Microsoft.Xna.Framework;
using System;

namespace SurvivalGame.Features.Player;

public sealed class PlayerModel
{
    private const int DefaultShieldLimit = 2;
    private const float DefaultSpeedMultiplier = 1f;

    public PlayerModel(Vector2 startPosition, float speedPixelsPerSecond, int maxHealth, int maxAmmo)
    {
        Position = startPosition;
        BaseSpeedPixelsPerSecond = speedPixelsPerSecond;
        MaxHealth = maxHealth;
        Health = maxHealth;
        MaxAmmo = maxAmmo;
        CurrentAmmo = maxAmmo;
        MaxShields = DefaultShieldLimit;
        SpeedMultiplier = DefaultSpeedMultiplier;
        
        // Initial weapon stats
        BulletDamage = 25;
        FireRate = 0.25f; // Seconds between shots
        IsAutomatic = false;
        ShootTimer = 0f;
    }

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public float BaseSpeedPixelsPerSecond { get; }
    public float SpeedMultiplier { get; private set; }
    public float SpeedBoostTimer { get; private set; }
    public float SpeedPixelsPerSecond => BaseSpeedPixelsPerSecond * SpeedMultiplier;

    public int MaxHealth { get; }
    public int Health { get; set; }
    public int MaxShields { get; }
    public int ShieldCount { get; private set; }

    public int MaxAmmo { get; }
    public int CurrentAmmo { get; set; }
    public bool IsReloading { get; set; }
    public float ReloadTimer { get; set; }

    // Weapon stats
    public int BulletDamage { get; set; }
    public float FireRate { get; set; }
    public bool IsAutomatic { get; set; }
    public float ShootTimer { get; set; }

    public void Reset(Vector2 startPosition)
    {
        Position = startPosition;
        Health = MaxHealth;
        ShieldCount = 0;
        SpeedMultiplier = DefaultSpeedMultiplier;
        SpeedBoostTimer = 0f;
        CurrentAmmo = MaxAmmo;
        IsReloading = false;
        BulletDamage = 25;
        FireRate = 0.25f;
        IsAutomatic = false;
        ShootTimer = 0f;
    }

    public void ApplyDamage(int amount)
    {
        int remainingDamage = amount;
        while (remainingDamage > 0 && ShieldCount > 0)
        {
            ShieldCount--;
            remainingDamage--;
        }

        if (remainingDamage > 0)
        {
            Health -= remainingDamage;
            if (Health < 0)
            {
                Health = 0;
            }
        }
    }

    public void AddHealth(int amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }

    public void AddShield(int amount)
    {
        ShieldCount = Math.Min(MaxShields, ShieldCount + amount);
    }

    public void InstantReload()
    {
        CurrentAmmo = MaxAmmo;
        IsReloading = false;
        ReloadTimer = 0f;
    }

    public void ActivateSpeedBoost(float multiplier, float durationSeconds)
    {
        SpeedMultiplier = multiplier;
        SpeedBoostTimer = durationSeconds;
    }

    public void UpdateTimedEffects(float dt)
    {
        if (SpeedBoostTimer <= 0f)
        {
            return;
        }

        SpeedBoostTimer -= dt;
        if (SpeedBoostTimer <= 0f)
        {
            SpeedBoostTimer = 0f;
            SpeedMultiplier = DefaultSpeedMultiplier;
        }
    }
}
