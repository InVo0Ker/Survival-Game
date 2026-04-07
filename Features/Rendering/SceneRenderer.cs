using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurvivalGame.Features.Player;
using SurvivalGame.Features.Enemies;

namespace SurvivalGame.Features.Rendering;

public sealed class SceneRenderer
{
    public void Draw(
        SpriteBatch spriteBatch,
        Texture2D backgroundTexture,
        Texture2D playerTexture,
        PlayerModel player,
        EnemyModel enemy,
        Texture2D[] enemyTextures,
        Rectangle viewportBounds)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        spriteBatch.Draw(backgroundTexture, destinationRectangle: viewportBounds, Color.White);

        // Draw Enemy
        Texture2D currentEnemyTexture = enemyTextures[enemy.CurrentFrame];
        Vector2 enemyOrigin = new(currentEnemyTexture.Width * 0.5f, currentEnemyTexture.Height * 0.5f);
        spriteBatch.Draw(
            currentEnemyTexture,
            position: enemy.Position,
            sourceRectangle: null,
            color: Color.White,
            rotation: enemy.Rotation,
            origin: enemyOrigin,
            scale: 0.1f,
            effects: SpriteEffects.None,
            layerDepth: 0f);

        // Draw Player
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
