using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tilemaps;

namespace FirstGame.GameObjects;

public class GameWorld
{
    public TilemapWorld TilemapWorld { get; }
    public SpriteFont Font { get; }
    public GameLevel? ActiveLevel { get; private set; }

    public GameWorld(TilemapWorld tilemapWorld, SpriteFont font)
    {
        TilemapWorld = tilemapWorld;
        Font = font;
    }

    public void LoadLevel(
        int index,
        GameWindow window,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch)
    {
        LoadLevel(
            TilemapWorld.Levels[index],
            window,
            graphicsDevice,
            spriteBatch);
    }

    public void LoadLevel(
        Tilemap tilemap,
        GameWindow window,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch)
    {
        ActiveLevel = new GameLevel(tilemap);
        ActiveLevel.Initialize(window, graphicsDevice, spriteBatch, Font);
    }

    public void UnloadLevel()
    {
    }

    public void Update(GameTime gameTime)
    {
        ActiveLevel?.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        ActiveLevel?.Draw(gameTime, spriteBatch);
    }
}