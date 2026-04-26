using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurvivalGame.Features.Player;
using SurvivalGame.Features.Rendering;
using SurvivalGame.Features.Enemies;
using System.Collections.Generic;

namespace SurvivalGame;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private PlayerModel _player = null!;
    private PlayerMovementSystem _playerMovementSystem = null!;
    private List<EnemyModel> _enemies = new();
    private EnemySystem _enemySystem = null!;
    private EnemySpawnSystem _spawnSystem = null!;
    private BulletSystem _bulletSystem = null!;
    private WeaponUpgradeSystem _upgradeSystem = null!;
    private UpgradeBoxModel _upgradeBox = new();
    private List<BulletModel> _bullets = new();
    private SceneRenderer _sceneRenderer = null!;
    private Texture2D _backgroundTexture = null!;
    private Texture2D _playerTexture = null!;
    private Texture2D[] _enemyWalkTextures = null!;
    private Texture2D[] _enemyAttackTextures = null!;
    private Texture2D _crosshairTexture = null!;
    private Texture2D _boxTexture = null!;
    private MouseState _lastMouseState;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        _player = new PlayerModel(new Vector2(400f, 260f), speedPixelsPerSecond: 220f, maxHealth: 100, maxAmmo: 30);
        _playerMovementSystem = new PlayerMovementSystem();

        _enemySystem = new EnemySystem();
        _spawnSystem = new EnemySpawnSystem();
        _bulletSystem = new BulletSystem();
        _upgradeSystem = new WeaponUpgradeSystem();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        string contentPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Content");

        _backgroundTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/background.jpg"));
        _playerTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/bk_player_assets/player_9mmhandgun.png"));
        _crosshairTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/update/update/effect/croshair.png"));
        _boxTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/update/update/Background/box.png"));

        _enemyWalkTextures = new Texture2D[9];
        for (int i = 0; i < 9; i++)
        {
            string path = System.IO.Path.Combine(contentPath, $"Sprites/tds-zombie-character-sprite/Zombies/PNG Animations/1LVL/Zombie4_male/Walk/Walk_{i:000}.png");
            _enemyWalkTextures[i] = Texture2D.FromFile(GraphicsDevice, path);
        }

        _enemyAttackTextures = new Texture2D[9];
        for (int i = 0; i < 9; i++)
        {
            string path = System.IO.Path.Combine(contentPath, $"Sprites/tds-zombie-character-sprite/Zombies/PNG Animations/1LVL/Zombie4_male/Attack/Attack_{i:000}.png");
            _enemyAttackTextures[i] = Texture2D.FromFile(GraphicsDevice, path);
        }

        _sceneRenderer = new SceneRenderer();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Rectangle viewportBounds = GraphicsDevice.Viewport.Bounds;

        if (_player.Health > 0)
        {
            _playerMovementSystem.Update(_player, gameTime, viewportBounds);
            _upgradeSystem.Update(_upgradeBox, _player, viewportBounds, gameTime);
            _bulletSystem.UpdateTimers(_player, gameTime);

            MouseState currentMouseState = Mouse.GetState();
            // Shooting logic
            bool wantToShoot = _player.IsAutomatic 
                ? currentMouseState.LeftButton == ButtonState.Pressed 
                : (currentMouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released);

            if (wantToShoot)
            {
                _bulletSystem.Shoot(_bullets, _player);
            }
            _lastMouseState = currentMouseState;

            _spawnSystem.Update(_enemies, viewportBounds, gameTime);
        }
        else
        {
            // Game Over logic - check for restart
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                RestartGame();
            }
        }

        // Update enemies and remove dead ones
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            _enemySystem.Update(_enemies[i], _player, gameTime);
            if (_enemies[i].IsDead)
            {
                _enemies.RemoveAt(i);
            }
        }

        _bulletSystem.Update(_bullets, _enemies, _enemySystem, gameTime, viewportBounds);

        base.Update(gameTime);
    }

    private void RestartGame()
    {
        _player.Reset(new Vector2(400f, 260f));
        _enemies.Clear();
        _bullets.Clear();
        _upgradeSystem.Reset(_upgradeBox);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        Rectangle viewportBounds = GraphicsDevice.Viewport.Bounds;
        _sceneRenderer.Draw(_spriteBatch, _backgroundTexture, _playerTexture, _player, _enemies, _enemyWalkTextures, _enemyAttackTextures, _crosshairTexture, _bullets, _upgradeBox, _boxTexture, viewportBounds);

        base.Draw(gameTime);
    }
}
