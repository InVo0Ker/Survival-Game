using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SurvivalGame.Features.Player;

public enum PlayerState
{
    Idle,
    Walking,
    Dead
}

public enum WeaponType
{
    Pistol,
    MachineGun,
    Rifle,
    GrenadeLauncher,
    Flamethrower
}

public sealed class PlayerWeapon
{
    public WeaponType Type { get; set; }
    public int BulletDamage { get; set; }
    public float FireRate { get; set; }
    public bool IsAutomatic { get; set; }
    public int MaxAmmo { get; set; }
    public int CurrentAmmo { get; set; }
    public float ReloadTime { get; set; }

    public PlayerWeapon(WeaponType type, int damage, float fireRate, bool isAuto, int maxAmmo, float reloadTime = 2.0f)
    {
        Type = type;
        BulletDamage = damage;
        FireRate = fireRate;
        IsAutomatic = isAuto;
        MaxAmmo = maxAmmo;
        CurrentAmmo = maxAmmo;
        ReloadTime = reloadTime;
    }
}

public sealed class PlayerModel
{
    private const int DefaultShieldLimit = 2;
    private const float DefaultSpeedMultiplier = 1f;

    public PlayerModel(Vector2 startPosition, float speedPixelsPerSecond, int maxHealth)
    {
        Position = startPosition;
        BaseSpeedPixelsPerSecond = speedPixelsPerSecond;
        MaxHealth = maxHealth;
        Health = maxHealth;
        MaxShields = DefaultShieldLimit;
        SpeedMultiplier = DefaultSpeedMultiplier;
        
        State = PlayerState.Idle;
        Inventory = new List<PlayerWeapon> { CreateDefaultWeapon() };
        CurrentWeaponIndex = 0;
        ShootTimer = 0f;
    }

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public float MoveRotation { get; set; }
    public float BaseSpeedPixelsPerSecond { get; }
    public float SpeedMultiplier { get; private set; }
    public float SpeedBoostTimer { get; private set; }
    public float SpeedPixelsPerSecond => BaseSpeedPixelsPerSecond * SpeedMultiplier;

    public int MaxHealth { get; }
    public int Health { get; set; }
    public int MaxShields { get; }
    public int ShieldCount { get; private set; }

    public PlayerState State { get; set; }
    public float AnimationTimer { get; set; }
    public int CurrentFrame { get; set; }
    public float DeathAnimationTimer { get; set; }

    public List<PlayerWeapon> Inventory { get; private set; }
    public int CurrentWeaponIndex { get; set; }
    public PlayerWeapon CurrentWeapon => Inventory[CurrentWeaponIndex];
    public float ShootTimer { get; set; }
    public bool IsReloading { get; set; }
    public float ReloadTimer { get; set; }
    public float MuzzleFlashTimer { get; set; }

    private PlayerWeapon CreateDefaultWeapon()
    {
        return new PlayerWeapon(WeaponType.Pistol, 25, 0.25f, false, 30);
    }

    public void Reset(Vector2 startPosition)
    {
        Position = startPosition;
        Health = MaxHealth;
        ShieldCount = 0;
        SpeedMultiplier = DefaultSpeedMultiplier;
        SpeedBoostTimer = 0f;
        State = PlayerState.Idle;
        Inventory = new List<PlayerWeapon> { CreateDefaultWeapon() };
        CurrentWeaponIndex = 0;
        ShootTimer = 0f;
        IsReloading = false;
        AnimationTimer = 0f;
        CurrentFrame = 0;
        DeathAnimationTimer = 0f;
    }

    public void ApplyDamage(int amount)
    {
        if (State == PlayerState.Dead) return;

        int remainingDamage = amount;
        while (remainingDamage > 0 && ShieldCount > 0)
        {
            ShieldCount--;
            remainingDamage--;
        }

        if (remainingDamage > 0)
        {
            Health -= remainingDamage;
            if (Health <= 0)
            {
                Health = 0;
                State = PlayerState.Dead;
                CurrentFrame = 0;
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
        CurrentWeapon.CurrentAmmo = CurrentWeapon.MaxAmmo;
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
        if (SpeedBoostTimer > 0f)
        {
            SpeedBoostTimer -= dt;
            if (SpeedBoostTimer <= 0f)
            {
                SpeedBoostTimer = 0f;
                SpeedMultiplier = DefaultSpeedMultiplier;
            }
        }

        if (MuzzleFlashTimer > 0f)
        {
            MuzzleFlashTimer -= dt;
        }
    }
}
