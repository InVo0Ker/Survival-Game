using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurvivalGame.Features.UI;

public sealed class MenuRenderer
{
    private Texture2D _pixel = null!;

    private void EnsurePixel(GraphicsDevice graphicsDevice)
    {
        if (_pixel == null)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }
    }

    public void DrawStartMenu(
        SpriteBatch spriteBatch,
        Texture2D menuBackgroundTexture,
        Texture2D playButtonTexture,
        Rectangle viewportBounds,
        Rectangle playButtonBounds)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(menuBackgroundTexture, viewportBounds, Color.White);
        spriteBatch.Draw(playButtonTexture, playButtonBounds, Color.White);
        spriteBatch.End();
    }

    public void DrawGameOverMenu(
        SpriteBatch spriteBatch,
        Texture2D menuBackgroundTexture,
        Texture2D retryButtonTexture,
        Rectangle viewportBounds,
        Rectangle retryButtonBounds)
    {
        EnsurePixel(spriteBatch.GraphicsDevice);
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(menuBackgroundTexture, viewportBounds, Color.White);
        // Darken the gameplay backdrop to emphasize retry action.
        spriteBatch.Draw(_pixel, viewportBounds, Color.Black * 0.35f);
        spriteBatch.Draw(retryButtonTexture, retryButtonBounds, Color.White);
        spriteBatch.End();
    }

    public void DrawPauseMenu(
        SpriteBatch spriteBatch,
        Texture2D pausePanelTexture,
        Texture2D menuButtonTexture,
        Texture2D backButtonTexture,
        Rectangle pausePanelBounds,
        Rectangle menuButtonBounds,
        Rectangle backButtonBounds)
    {
        EnsurePixel(spriteBatch.GraphicsDevice);
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(_pixel, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.Black * 0.35f);
        spriteBatch.Draw(pausePanelTexture, pausePanelBounds, Color.White);
        spriteBatch.Draw(menuButtonTexture, menuButtonBounds, Color.White);
        spriteBatch.Draw(backButtonTexture, backButtonBounds, Color.White);
        spriteBatch.End();
    }
}
