using System.IO;
using System.Linq;
using Core;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MonoGame.Extended.ViewportAdapters;

namespace FirstGame.Screens;

public class GameplayScreen : BaseScreen
{
    private TilemapWorld _world = null!;
    private Tilemap _tilemap = null!;
    private OrthographicCamera _camera = null!;
    private TilemapSpriteBatchRenderer _renderer = null!;
    private Vector2 _spawnPoint = Vector2.Zero;

    public GameplayScreen(BaseGame game) : base(game)
    {
    }

    public override void LoadContent()
    {
        base.LoadContent();

        _renderer = new TilemapSpriteBatchRenderer();

        _world = Content.Load<TilemapWorld>(Path.Combine("map", "world"));
        _tilemap = _world.Levels[0];
        _renderer.LoadTilemap(_tilemap);

        _spawnPoint = (_tilemap.Layers["PlayerEntities"] as TilemapObjectLayer)?
            .Objects.First(x => x.Name is "SpawnPoint").Position ?? Vector2.Zero;
    }

    public override void Initialize()
    {
        base.Initialize();

        var viewportAdapter = new BoxingViewportAdapter(
            Window,
            GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight);
        _camera = new OrthographicCamera(viewportAdapter);
    }

    public override void Update(GameTime gameTime)
    {
        _renderer.Update(gameTime);
        _camera.LookAt(_spawnPoint);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_tilemap.BackgroundColor ?? Color.Black);

        _renderer.Draw(SpriteBatch, _camera);

        SpriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
        
        SpriteBatch.DrawRectangle(
            new RectangleF(_spawnPoint.X, _spawnPoint.Y, 16, 16),
            Color.White);

        SpriteBatch.End();
    }
}