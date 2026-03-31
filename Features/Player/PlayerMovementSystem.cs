using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SurvivalGame.Features.Player;

public sealed class PlayerMovementSystem
{
    public void Update(PlayerModel player, GameTime gameTime, Rectangle worldBounds)
    {
        KeyboardState keyboard = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W))
        {
            direction.Y -= 1f;
        }

        if (keyboard.IsKeyDown(Keys.S))
        {
            direction.Y += 1f;
        }

        if (keyboard.IsKeyDown(Keys.A))
        {
            direction.X -= 1f;
        }

        if (keyboard.IsKeyDown(Keys.D))
        {
            direction.X += 1f;
        }

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        player.Position += direction * player.SpeedPixelsPerSecond * dt;
        player.Position = Vector2.Clamp(
            player.Position,
            new Vector2(worldBounds.Left, worldBounds.Top),
            new Vector2(worldBounds.Right, worldBounds.Bottom));
    }
}
