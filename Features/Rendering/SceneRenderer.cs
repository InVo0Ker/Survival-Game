using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurvivalGame.Features.Player;

namespace SurvivalGame.Features.Rendering;

public sealed class SceneRenderer
{
    public void Draw(
        SpriteBatch spriteBatch,
        Texture2D backgroundTexture,
        Texture2D playerTexture,
        PlayerModel player,
        Rectangle viewportBounds)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        spriteBatch.Draw(backgroundTexture, destinationRectangle: viewportBounds, Color.White);

        Vector2 playerOrigin = new(playerTexture.Width * 0.5f, playerTexture.Height * 0.5f);
        spriteBatch.Draw(
            playerTexture,
            position: player.Position,
            sourceRectangle: null,
            color: Color.White,
            rotation: 0f,
            origin: playerOrigin,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0f);

        spriteBatch.End();
    }
}
