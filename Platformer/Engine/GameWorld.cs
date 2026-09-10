using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tilemaps;

namespace Platformer.Engine;

/// <summary>
/// Holds the tilemap world describing the game levels and manages the currently
/// active level.
/// </summary>
public class GameWorld
{
    /// <summary>
    /// Gets the tilemap world containing all the levels.
    /// </summary>
    public TilemapWorld World { get; }

    /// <summary>
    /// Gets the font used to render text within the levels.
    /// </summary>
    public SpriteFont Font { get; }

    /// <summary>
    /// Gets the currently active level, or <see langword="null"/> when no level
    /// is loaded.
    /// </summary>
    public GameLevel? ActiveLevel { get; private set; }

    /// <summary>
    /// Initializes a new instance of the game world.
    /// </summary>
    /// <param name="world">The tilemap world containing all the levels.</param>
    /// <param name="font">The font used to render text within the levels.</param>
    public GameWorld(TilemapWorld world, SpriteFont font)
    {
        World = world;
        Font = font;
    }

    /// <summary>
    /// Loads the level at the given index of the tilemap world and makes it active.
    /// </summary>
    /// <param name="index">The zero-based index of the level to load.</param>
    /// <param name="window">The game window used by the camera viewport adapter.</param>
    /// <param name="graphicsDevice">The graphics device used to create rendering resources.</param>
    /// <param name="content">The content manager used to load level assets.</param>
    /// <param name="spriteBatch">The sprite batch used to render the level.</param>
    public void LoadLevel(
        int index,
        GameWindow window,
        GraphicsDevice graphicsDevice,
        ContentManager content,
        SpriteBatch spriteBatch)
    {
        LoadLevel(
            World.Levels[index],
            window,
            graphicsDevice,
            content,
            spriteBatch);
    }

    /// <summary>
    /// Loads the given tilemap as a level and makes it active.
    /// </summary>
    /// <param name="tilemap">The tilemap describing the level to load.</param>
    /// <param name="window">The game window used by the camera viewport adapter.</param>
    /// <param name="graphicsDevice">The graphics device used to create rendering resources.</param>
    /// <param name="content">The content manager used to load level assets.</param>
    /// <param name="spriteBatch">The sprite batch used to render the level.</param>
    public void LoadLevel(
        Tilemap tilemap,
        GameWindow window,
        GraphicsDevice graphicsDevice,
        ContentManager content,
        SpriteBatch spriteBatch)
    {
        ActiveLevel = new GameLevel(tilemap);
        ActiveLevel.Initialize(window, graphicsDevice, content, spriteBatch);
    }

    /// <summary>
    /// Updates the active level.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public void Update(GameTime gameTime)
    {
        ActiveLevel?.Update(gameTime);
    }

    /// <summary>
    /// Draws the active level.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    /// <param name="spriteBatch">The sprite batch used to render the level.</param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        ActiveLevel?.Draw(gameTime, spriteBatch);
    }
}