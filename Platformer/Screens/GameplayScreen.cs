using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MonoGame.Extended.ViewportAdapters;
using Platformer.Engine.Helpers;
using Platformer.GameObjects;
using Platformer.GameObjects.Levels;

namespace Platformer.Screens;

public class GameplayScreen(Game game) : GameScreen(game)
{
    private SpriteBatch _spriteBatch = null!;
    private Sprite _playerSprite = null!;
    private Tilemap _map = null!;
    private OrthographicCamera _camera = null!;
    private GameLevel _level = null!;
    private TilemapSpriteBatchRenderer _renderer = null!;

    public override void Initialize()
    {
        base.Initialize();

        var boxingViewportAdapter = new BoxingViewportAdapter(
            window: Game.Window,
            graphicsDevice: GraphicsDevice,
            virtualWidth: 640,
            virtualHeight: 320);

        _renderer = new TilemapSpriteBatchRenderer
        {
            BlendState = BlendState.AlphaBlend,
        };
        _camera = new OrthographicCamera(boxingViewportAdapter);
    }

    /// <summary>
    /// Loads the game content, creates the game world and loads the first level.
    /// </summary>
    public override void LoadContent()
    {
        base.LoadContent();

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _playerSprite = SpriteHelper.CreateAnimatedSprite(Content, Path.Combine("texture", "player", "player"));
        _map = Content.Load<TilemapWorld>(Path.Combine("map", "world")).Levels[0];
        _level = new GameLevel(_map, _camera, _spriteBatch);

        _camera.EnableWorldBounds(_map.WorldBounds);
        _camera.IsZoomClampedToWorldBounds = true;

        _renderer.LoadTilemap(_map);

        _level.AddPlayer(_level.LevelProperties.SpawnPoint[0], _playerSprite);
    }

    /// <summary>
    /// Updates the active game world.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Update(GameTime gameTime)
    {
        _level.Update(gameTime);
    }

    /// <summary>
    /// Clears the back buffer and draws the active game world.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_map.BackgroundColor ?? Color.Black);

        _renderer.Draw(_spriteBatch, _camera);
        _level.Draw(gameTime);
    }
}