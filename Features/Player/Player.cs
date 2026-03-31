using Microsoft.Xna.Framework;

namespace SurvivalGame.Features.Player;

public sealed class PlayerModel
{
    public PlayerModel(Vector2 startPosition, float speedPixelsPerSecond)
    {
        Position = startPosition;
        SpeedPixelsPerSecond = speedPixelsPerSecond;
    }

    public Vector2 Position { get; set; }

    public float SpeedPixelsPerSecond { get; }
}
