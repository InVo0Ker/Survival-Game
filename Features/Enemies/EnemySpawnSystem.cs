using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SurvivalGame.Features.Enemies;

public sealed class EnemySpawnSystem
{
    private double _spawnTimer;
    private const double SpawnInterval = 1.0; // seconds
    private readonly Random _random = new();

    public void Update(List<EnemyModel> enemies, Rectangle viewportBounds, GameTime gameTime)
    {
        _spawnTimer += gameTime.ElapsedGameTime.TotalSeconds;

        if (_spawnTimer >= SpawnInterval)
        {
            _spawnTimer -= SpawnInterval;
            enemies.Add(SpawnEnemy(viewportBounds));
        }
    }

    private EnemyModel SpawnEnemy(Rectangle viewportBounds)
    {
        // Pick a random edge to spawn on
        int edge = _random.Next(4);
        Vector2 spawnPos = Vector2.Zero;
        float offset = 50f;

        switch (edge)
        {
            case 0: // Top
                spawnPos = new Vector2(_random.Next(viewportBounds.Left, viewportBounds.Right), viewportBounds.Top - offset);
                break;
            case 1: // Bottom
                spawnPos = new Vector2(_random.Next(viewportBounds.Left, viewportBounds.Right), viewportBounds.Bottom + offset);
                break;
            case 2: // Left
                spawnPos = new Vector2(viewportBounds.Left - offset, _random.Next(viewportBounds.Top, viewportBounds.Bottom));
                break;
            case 3: // Right
                spawnPos = new Vector2(viewportBounds.Right + offset, _random.Next(viewportBounds.Top, viewportBounds.Bottom));
                break;
        }

        // Randomly pick a zombie type:
        // LVL1 Melee (90% chance)
        // LVL2 Ranged (10% chance)
        int roll = _random.Next(100);
        int typeIndex;
        
        if (roll < 90) // LVL1
        {
            typeIndex = _random.Next(4);
        }
        else // LVL2
        {
            typeIndex = _random.Next(4, 6);
        }
        
        float speed;
        int damage;
        float attackRange;
        float attackCooldown;
        int maxHealth;
        EnemyCategory category;
        EnemyWeapon weapon = EnemyWeapon.None;

        if (typeIndex < 4) // LVL1 Melee
        {
            speed = _random.Next(80, 120);
            damage = 1;
            attackRange = 40f;
            attackCooldown = 1.0f;
            maxHealth = 100;
            category = EnemyCategory.Melee;
        }
        else // LVL2 Ranged
        {
            speed = _random.Next(50, 80);
            maxHealth = 150;
            category = EnemyCategory.Ranged;

            if (typeIndex == 4) // Army Zombie
            {
                int weaponRoll = _random.Next(3);
                if (weaponRoll == 0) // Pistol
                {
                    weapon = EnemyWeapon.Pistol;
                    damage = 1;
                    attackRange = 300f;
                    attackCooldown = 1.5f;
                }
                else if (weaponRoll == 1) // Rifle
                {
                    weapon = EnemyWeapon.Rifle;
                    damage = 1; // Damage per bullet in burst
                    attackRange = 400f;
                    attackCooldown = 2.5f;
                }
                else // Sniper
                {
                    weapon = EnemyWeapon.Sniper;
                    damage = 3;
                    attackRange = 600f;
                    attackCooldown = 4.0f;
                }
            }
            else // Cop Zombie
            {
                int weaponRoll = _random.Next(3);
                if (weaponRoll == 0) // Pistol
                {
                    weapon = EnemyWeapon.Pistol;
                    damage = 1;
                    attackRange = 300f;
                    attackCooldown = 1.5f;
                }
                else if (weaponRoll == 1) // Rifle
                {
                    weapon = EnemyWeapon.Rifle;
                    damage = 1;
                    attackRange = 400f;
                    attackCooldown = 2.5f;
                }
                else // Shotgun
                {
                    weapon = EnemyWeapon.Shotgun;
                    damage = 1; // Damage per pellet
                    attackRange = 250f;
                    attackCooldown = 3.0f;
                }
            }
        }

        return new EnemyModel(typeIndex, spawnPos, speed, damage, attackRange, attackCooldown, maxHealth, category, weapon);
    }
}
