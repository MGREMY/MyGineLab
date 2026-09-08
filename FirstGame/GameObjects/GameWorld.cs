using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tilemaps;

namespace FirstGame.GameObjects;

public class GameWorld
{
    public TilemapWorld TilemapWorld { get; }
    public GameLevel? ActiveLevel { get; private set; }

    public GameWorld(TilemapWorld tilemapWorld)
    {
        TilemapWorld = tilemapWorld;
    }

    public void LoadLevel(int index, GameWindow window, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        LoadLevel(TilemapWorld.Levels[index], window, graphicsDevice, spriteBatch);
    }

    public void LoadLevel(Tilemap tilemap, GameWindow window, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        ActiveLevel = new GameLevel(tilemap);
        ActiveLevel.Initialize(window, graphicsDevice, spriteBatch);
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