using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurvivalGame.Features.Player;
using SurvivalGame.Features.Rendering;
using SurvivalGame.Features.Enemies;

namespace SurvivalGame;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private PlayerModel _player = null!;
    private PlayerMovementSystem _playerMovementSystem = null!;
    private EnemyModel _enemy = null!;
    private EnemySystem _enemySystem = null!;
    private SceneRenderer _sceneRenderer = null!;
    private Texture2D _backgroundTexture = null!;
    private Texture2D _playerTexture = null!;
    private Texture2D[] _enemyTextures = null!;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _player = new PlayerModel(new Vector2(400f, 260f), speedPixelsPerSecond: 220f);
        _playerMovementSystem = new PlayerMovementSystem();

        _enemy = new EnemyModel(new Vector2(100f, 100f), speedPixelsPerSecond: 100f);
        _enemySystem = new EnemySystem();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _backgroundTexture = Content.Load<Texture2D>("Sprites/background");
        _playerTexture = Content.Load<Texture2D>("Sprites/bk_player_assets/player_9mmhandgun");

        _enemyTextures = new Texture2D[9];
        for (int i = 0; i < 9; i++)
        {
            _enemyTextures[i] = Content.Load<Texture2D>($"Sprites/tds-zombie-character-sprite/Zombies/PNG Animations/1LVL/Zombie4_male/Walk/Walk_{i:000}");
        }

        _sceneRenderer = new SceneRenderer();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Rectangle viewportBounds = GraphicsDevice.Viewport.Bounds;
        _playerMovementSystem.Update(_player, gameTime, viewportBounds);
        _enemySystem.Update(_enemy, _player, gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        Rectangle viewportBounds = GraphicsDevice.Viewport.Bounds;
        _sceneRenderer.Draw(_spriteBatch, _backgroundTexture, _playerTexture, _player, _enemy, _enemyTextures, viewportBounds);

        base.Draw(gameTime);
    }
}
