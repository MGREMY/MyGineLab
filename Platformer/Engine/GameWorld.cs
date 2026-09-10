using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tilemaps;

namespace Platformer.Engine;

public class GameWorld
{
    public TilemapWorld World { get; }
    public SpriteFont Font { get; }
    public GameLevel? ActiveLevel { get; private set; }

    public GameWorld(TilemapWorld world, SpriteFont font)
    {
        World = world;
        Font = font;
    }

    public void LoadLevel(
        int index,
        GameWindow window,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch)
    {
        LoadLevel(
            World.Levels[index],
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
        ActiveLevel.Initialize(window, graphicsDevice, spriteBatch);
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