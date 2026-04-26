using Microsoft.Xna.Framework;

namespace SurvivalGame.Features.Player;

public sealed class PlayerModel
{
    public PlayerModel(Vector2 startPosition, float speedPixelsPerSecond, int maxHealth, int maxAmmo)
    {
        Position = startPosition;
        SpeedPixelsPerSecond = speedPixelsPerSecond;
        MaxHealth = maxHealth;
        Health = maxHealth;
        MaxAmmo = maxAmmo;
        CurrentAmmo = maxAmmo;
        
        // Initial weapon stats
        BulletDamage = 25;
        FireRate = 0.25f; // Seconds between shots
        IsAutomatic = false;
        ShootTimer = 0f;
    }

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public float SpeedPixelsPerSecond { get; }

    public int MaxHealth { get; }
    public int Health { get; set; }

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
        CurrentAmmo = MaxAmmo;
        IsReloading = false;
        BulletDamage = 25;
        FireRate = 0.25f;
        IsAutomatic = false;
        ShootTimer = 0f;
    }
}
