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

        float speed = _random.Next(80, 120);
        return new EnemyModel(spawnPos, speed, damage: 10, attackRange: 40f, attackCooldown: 1.0f, maxHealth: 100);
    }
}
