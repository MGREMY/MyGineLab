using System.IO;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tilemaps;
using Platformer.Engine.Helpers;
using Platformer.GameObjects.Worlds;

namespace Platformer.Screens;

/// <summary>
/// The gameplay screen that owns the active <see cref="GameWorld"/> and forwards
/// the update and draw calls to it.
/// </summary>
public class GameplayScreen(BaseGame game) : BaseScreen(game)
{
    private GameWorld _gameWorld = null!;

    /// <summary>
    /// Loads the game content, creates the game world and loads the first level.
    /// </summary>
    public override void LoadContent()
    {
        base.LoadContent();

        var playerSprite = SpriteHelper.CreateAnimatedSprite(Content, Path.Combine("texture", "player", "player"));
        var tilemapWorld = Content.Load<TilemapWorld>(Path.Combine("map", "world"));

        _gameWorld = new GameWorld(tilemapWorld, playerSprite, Window, GraphicsDevice, SpriteBatch);
        _gameWorld.LoadLevel(0);
    }

    /// <summary>
    /// Updates the active game world.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Update(GameTime gameTime)
    {
        _gameWorld.Update(gameTime);
    }

    /// <summary>
    /// Clears the back buffer and draws the active game world.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_gameWorld.ActiveLevel?.Tilemap.BackgroundColor ?? Color.Black);

        _gameWorld.Draw(gameTime);
    }
}