using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurvivalGame.Features.Player;
using SurvivalGame.Features.Rendering;

namespace SurvivalGame;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private PlayerModel _player = null!;
    private PlayerMovementSystem _playerMovementSystem = null!;
    private SceneRenderer _sceneRenderer = null!;
    private Texture2D _backgroundTexture = null!;
    private Texture2D _playerTexture = null!;

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _backgroundTexture = Content.Load<Texture2D>("Sprites/background");
        _playerTexture = Content.Load<Texture2D>("Sprites/bk_player_assets/player_9mmhandgun");
        _sceneRenderer = new SceneRenderer();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Rectangle viewportBounds = GraphicsDevice.Viewport.Bounds;
        _playerMovementSystem.Update(_player, gameTime, viewportBounds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        Rectangle viewportBounds = GraphicsDevice.Viewport.Bounds;
        _sceneRenderer.Draw(_spriteBatch, _backgroundTexture, _playerTexture, _player, viewportBounds);

        base.Draw(gameTime);
    }
}
