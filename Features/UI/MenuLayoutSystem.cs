using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurvivalGame.Features.UI;

public sealed class MenuLayoutSystem
{
    public Rectangle GetCenteredButton(Texture2D texture, Rectangle viewportBounds)
    {
        return new Rectangle(
            viewportBounds.Center.X - texture.Width / 2,
            viewportBounds.Center.Y - texture.Height / 2,
            texture.Width,
            texture.Height);
    }

    public Rectangle GetPausePanel(Texture2D pausePanelTexture, Rectangle viewportBounds)
    {
        return new Rectangle(
            viewportBounds.Center.X - pausePanelTexture.Width / 2,
            viewportBounds.Center.Y - pausePanelTexture.Height / 2,
            pausePanelTexture.Width,
            pausePanelTexture.Height);
    }

    public Rectangle GetPauseMenuButton(Texture2D menuButtonTexture, Rectangle pausePanel)
    {
        return new Rectangle(
            pausePanel.Center.X - menuButtonTexture.Width / 2,
            pausePanel.Y + 65,
            menuButtonTexture.Width,
            menuButtonTexture.Height);
    }

    public Rectangle GetPauseBackButton(Texture2D backButtonTexture, Rectangle menuButtonBounds)
    {
        const int verticalGap = 35;
        return new Rectangle(
            menuButtonBounds.Center.X - backButtonTexture.Width / 2,
            menuButtonBounds.Bottom + verticalGap,
            backButtonTexture.Width,
            backButtonTexture.Height);
    }
}
