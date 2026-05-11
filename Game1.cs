using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurvivalGame.Features.GameFlow;
using SurvivalGame.Features.Player;
using SurvivalGame.Features.Rendering;
using SurvivalGame.Features.Enemies;
using SurvivalGame.Features.UI;
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
    private EnemyDropSystem _enemyDropSystem = null!;
    private BulletSystem _bulletSystem = null!;
    private WeaponUpgradeSystem _upgradeSystem = null!;
    private UpgradeBoxModel _upgradeBox = new();
    private List<BulletModel> _bullets = new();
    private List<EnemyDropModel> _enemyDrops = new();
    private SceneRenderer _sceneRenderer = null!;
    private Texture2D _backgroundTexture = null!;
    private Texture2D _playerTexture = null!;
    private Texture2D[][] _allEnemyWalkTextures = null!;
    private Texture2D[][] _allEnemyAttackTextures = null!;
    private Texture2D _crosshairTexture = null!;
    private Texture2D _boxTexture = null!;
    private Texture2D _healthIconTexture = null!;
    private Texture2D _ammoDropTexture = null!;
    private Texture2D _armorDropTexture = null!;
    private Texture2D _speedDropTexture = null!;
    private Texture2D _menuBackgroundTexture = null!;
    private Texture2D _playButtonTexture = null!;
    private Texture2D _retryButtonTexture = null!;
    private Texture2D _pausePanelTexture = null!;
    private Texture2D _pauseMenuButtonTexture = null!;
    private Texture2D _pauseBackButtonTexture = null!;
    private MenuRenderer _menuRenderer = null!;
    private MenuLayoutSystem _menuLayoutSystem = null!;
    private GameLoopState _gameState;
    private MouseState _lastMouseState;
    private KeyboardState _lastKeyboardState;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        _player = new PlayerModel(new Vector2(400f, 260f), speedPixelsPerSecond: 220f, maxHealth: 5, maxAmmo: 30);
        _playerMovementSystem = new PlayerMovementSystem();

        _enemySystem = new EnemySystem();
        _spawnSystem = new EnemySpawnSystem();
        _enemyDropSystem = new EnemyDropSystem();
        _bulletSystem = new BulletSystem();
        _upgradeSystem = new WeaponUpgradeSystem();
        _menuRenderer = new MenuRenderer();
        _menuLayoutSystem = new MenuLayoutSystem();
        _gameState = GameLoopState.StartMenu;

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
        _healthIconTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/bk_player_assets/charBar/HP Icon.png"));
        _ammoDropTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/enemyDrop/Ammo Box.png"));
        _armorDropTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/enemyDrop/Armor Icon.png"));
        _speedDropTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/enemyDrop/Iocn_Speed_01.png"));
        _menuBackgroundTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/gameFlow/Background.png"));
        _playButtonTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/gameFlow/BTN PLAY.png"));
        _retryButtonTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/gameFlow/BTN Retry.png"));
        _pausePanelTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/gameFlow/PAUSE PRESET.png"));
        _pauseMenuButtonTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/gameFlow/BTN MENU.png"));
        _pauseBackButtonTexture = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, "Sprites/gameFlow/BTN BACK.png"));

        string[] enemyPaths = new string[]
        {
            "Sprites/tds-zombie-character-sprite/Zombies/PNG Animations/1LVL/Zombie1_female",
            "Sprites/tds-zombie-character-sprite/Zombies/PNG Animations/1LVL/Zombie2_female",
            "Sprites/tds-zombie-character-sprite/Zombies/PNG Animations/1LVL/Zombie3_male",
            "Sprites/tds-zombie-character-sprite/Zombies/PNG Animations/1LVL/Zombie4_male",
            "Sprites/tds-zombie-character-sprite/Zombies/PNG Animations/2LVL/Army_zombie",
            "Sprites/tds-zombie-character-sprite/Zombies/PNG Animations/2LVL/Cop_Zombie"
        };

        _allEnemyWalkTextures = new Texture2D[enemyPaths.Length][];
        _allEnemyAttackTextures = new Texture2D[enemyPaths.Length][];

        for (int t = 0; t < enemyPaths.Length; t++)
        {
            _allEnemyWalkTextures[t] = new Texture2D[9];
            for (int i = 0; i < 9; i++)
            {
                string path = System.IO.Path.Combine(contentPath, $"{enemyPaths[t]}/Walk/Walk_{i:000}.png");
                _allEnemyWalkTextures[t][i] = Texture2D.FromFile(GraphicsDevice, path);
            }

            if (t < 4) // Melee
            {
                _allEnemyAttackTextures[t] = new Texture2D[9];
                for (int i = 0; i < 9; i++)
                {
                    string path = System.IO.Path.Combine(contentPath, $"{enemyPaths[t]}/Attack/Attack_{i:000}.png");
                    _allEnemyAttackTextures[t][i] = Texture2D.FromFile(GraphicsDevice, path);
                }
            }
            else // Ranged (Load weapon-specific single-frame textures)
            {
                _allEnemyAttackTextures[t] = new Texture2D[5]; // Indices mapping to EnemyWeapon enum
                _allEnemyAttackTextures[t][(int)EnemyWeapon.Pistol] = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, $"{enemyPaths[t]}/Attack/attack_pistol.png"));
                _allEnemyAttackTextures[t][(int)EnemyWeapon.Rifle] = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, $"{enemyPaths[t]}/Attack/attack_rifle.png"));
                
                if (t == 4) // Army Sniper
                    _allEnemyAttackTextures[t][(int)EnemyWeapon.Sniper] = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, $"{enemyPaths[t]}/Attack/attack_sniper.png"));
                else // Cop Shotgun
                    _allEnemyAttackTextures[t][(int)EnemyWeapon.Shotgun] = Texture2D.FromFile(GraphicsDevice, System.IO.Path.Combine(contentPath, $"{enemyPaths[t]}/Attack/attack_shotgun.png"));
            }
        }

        _sceneRenderer = new SceneRenderer();
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            Exit();

        Rectangle viewportBounds = GraphicsDevice.Viewport.Bounds;
        MouseState currentMouseState = Mouse.GetState();

        switch (_gameState)
        {
            case GameLoopState.StartMenu:
                IsMouseVisible = true;
                HandleStartMenuInput(currentMouseState, viewportBounds);
                break;
            case GameLoopState.Playing:
                IsMouseVisible = false;
                HandlePauseToggle(keyboardState);
                if (_gameState == GameLoopState.Playing)
                {
                    UpdateGameplay(gameTime, currentMouseState, viewportBounds);
                }
                break;
            case GameLoopState.Paused:
                IsMouseVisible = true;
                HandlePauseMenuInput(currentMouseState, viewportBounds);
                break;
            case GameLoopState.GameOver:
                IsMouseVisible = true;
                HandleGameOverInput(currentMouseState, viewportBounds);
                break;
        }

        _lastMouseState = currentMouseState;
        _lastKeyboardState = keyboardState;
        base.Update(gameTime);
    }

    private void HandlePauseToggle(KeyboardState keyboardState)
    {
        bool isEscPressedNow = keyboardState.IsKeyDown(Keys.Escape);
        bool wasEscPressedBefore = _lastKeyboardState.IsKeyDown(Keys.Escape);
        if (isEscPressedNow && !wasEscPressedBefore)
        {
            _gameState = GameLoopState.Paused;
        }
    }

    private void UpdateGameplay(GameTime gameTime, MouseState currentMouseState, Rectangle viewportBounds)
    {
        _playerMovementSystem.Update(_player, gameTime, viewportBounds);
        _upgradeSystem.Update(_upgradeBox, _player, viewportBounds, gameTime);
        _bulletSystem.UpdateTimers(_player, gameTime);

        bool wantToShoot = _player.IsAutomatic
            ? currentMouseState.LeftButton == ButtonState.Pressed
            : IsNewLeftClick(currentMouseState);

        if (wantToShoot)
        {
            _bulletSystem.Shoot(_bullets, _player);
        }

        _spawnSystem.Update(_enemies, viewportBounds, gameTime);

        // Update enemies and remove dead ones
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            _enemySystem.Update(_enemies[i], _player, _bullets, gameTime);
            if (_enemies[i].IsDead)
            {
                _enemyDropSystem.TrySpawnOnEnemyDeath(_enemyDrops, _enemies[i].Position);
                _enemies.RemoveAt(i);
            }
        }

        _bulletSystem.Update(_bullets, _enemies, _enemySystem, _player, gameTime, viewportBounds);
        _enemyDropSystem.UpdateCollection(_enemyDrops, _player, gameTime);

        if (_player.Health <= 0)
        {
            _gameState = GameLoopState.GameOver;
        }
    }

    private void HandleStartMenuInput(MouseState currentMouseState, Rectangle viewportBounds)
    {
        Rectangle playButtonBounds = _menuLayoutSystem.GetCenteredButton(_playButtonTexture, viewportBounds);
        if (IsNewLeftClick(currentMouseState) && playButtonBounds.Contains(currentMouseState.Position))
        {
            RestartGame();
            _gameState = GameLoopState.Playing;
        }
    }

    private void HandleGameOverInput(MouseState currentMouseState, Rectangle viewportBounds)
    {
        Rectangle retryButtonBounds = _menuLayoutSystem.GetCenteredButton(_retryButtonTexture, viewportBounds);

        if (IsNewLeftClick(currentMouseState) && retryButtonBounds.Contains(currentMouseState.Position))
        {
            RestartGame();
            _gameState = GameLoopState.Playing;
        }
    }

    private void HandlePauseMenuInput(MouseState currentMouseState, Rectangle viewportBounds)
    {
        Rectangle pausePanelBounds = _menuLayoutSystem.GetPausePanel(_pausePanelTexture, viewportBounds);
        Rectangle menuButtonBounds = _menuLayoutSystem.GetPauseMenuButton(_pauseMenuButtonTexture, pausePanelBounds);
        Rectangle backButtonBounds = _menuLayoutSystem.GetPauseBackButton(_pauseBackButtonTexture, menuButtonBounds);

        if (IsNewLeftClick(currentMouseState) && menuButtonBounds.Contains(currentMouseState.Position))
        {
            RestartGame();
            _gameState = GameLoopState.StartMenu;
        }
        else if (IsNewLeftClick(currentMouseState) && backButtonBounds.Contains(currentMouseState.Position))
        {
            _gameState = GameLoopState.Playing;
        }
    }

    private bool IsNewLeftClick(MouseState currentMouseState)
    {
        return currentMouseState.LeftButton == ButtonState.Pressed &&
               _lastMouseState.LeftButton == ButtonState.Released;
    }

    private void RestartGame()
    {
        _player.Reset(new Vector2(400f, 260f));
        _enemies.Clear();
        _bullets.Clear();
        _enemyDrops.Clear();
        _upgradeSystem.Reset(_upgradeBox);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        Rectangle viewportBounds = GraphicsDevice.Viewport.Bounds;

        switch (_gameState)
        {
            case GameLoopState.StartMenu:
            {
                Rectangle playButtonBounds = _menuLayoutSystem.GetCenteredButton(_playButtonTexture, viewportBounds);
                _menuRenderer.DrawStartMenu(_spriteBatch, _menuBackgroundTexture, _playButtonTexture, viewportBounds, playButtonBounds);
                break;
            }
            case GameLoopState.Playing:
                _sceneRenderer.Draw(_spriteBatch, _backgroundTexture, _playerTexture, _player, _enemies, _allEnemyWalkTextures, _allEnemyAttackTextures, _crosshairTexture, _bullets, _enemyDrops, _upgradeBox, _boxTexture, _healthIconTexture, _ammoDropTexture, _armorDropTexture, _speedDropTexture, viewportBounds);
                break;
            case GameLoopState.Paused:
            {
                _sceneRenderer.Draw(_spriteBatch, _backgroundTexture, _playerTexture, _player, _enemies, _allEnemyWalkTextures, _allEnemyAttackTextures, _crosshairTexture, _bullets, _enemyDrops, _upgradeBox, _boxTexture, _healthIconTexture, _ammoDropTexture, _armorDropTexture, _speedDropTexture, viewportBounds);
                Rectangle pausePanelBounds = _menuLayoutSystem.GetPausePanel(_pausePanelTexture, viewportBounds);
                Rectangle menuButtonBounds = _menuLayoutSystem.GetPauseMenuButton(_pauseMenuButtonTexture, pausePanelBounds);
                Rectangle backButtonBounds = _menuLayoutSystem.GetPauseBackButton(_pauseBackButtonTexture, menuButtonBounds);
                _menuRenderer.DrawPauseMenu(_spriteBatch, _pausePanelTexture, _pauseMenuButtonTexture, _pauseBackButtonTexture, pausePanelBounds, menuButtonBounds, backButtonBounds);
                break;
            }
            case GameLoopState.GameOver:
            {
                Rectangle retryButtonBounds = _menuLayoutSystem.GetCenteredButton(_retryButtonTexture, viewportBounds);
                _menuRenderer.DrawGameOverMenu(_spriteBatch, _menuBackgroundTexture, _retryButtonTexture, viewportBounds, retryButtonBounds);
                break;
            }
        }

        base.Draw(gameTime);
    }
}
