using System.IO;
using Core;
using FirstGame.Ecs.Components;
using FirstGame.Ecs.Systems;
using FirstGame.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MonoGame.Extended.ViewportAdapters;

namespace FirstGame.Screens;

public class GameplayScreen : BaseScreen
{
    private readonly TilemapSpriteBatchRenderer _renderer = new();
    private OrthographicCamera _camera = null!;
    private World _ecsWorld = null!;
    private GameWorld _gameWorld = null!;

    public GameplayScreen(BaseGame game) : base(game)
    {
    }

    public override void Initialize()
    {
        base.Initialize();

        var viewportAdapter = new BoxingViewportAdapter(
            Window,
            GraphicsDevice,
            640,
            320);
        _camera = new OrthographicCamera(viewportAdapter);
    }

    public override void LoadContent()
    {
        base.LoadContent();

        var tilemapWorld = Content.Load<TilemapWorld>(Path.Combine("map", "world"));

        _gameWorld = GameWorld.FromTilemapWorld(tilemapWorld);

        _renderer.LoadTilemap(_gameWorld.CurrentGameLevel.Tilemap);

        _camera.EnableWorldBounds(_gameWorld.CurrentGameLevel.Tilemap.WorldBounds);
        _camera.IsZoomClampedToWorldBounds = true;

        _ecsWorld = new WorldBuilder()
            .AddSystem(new InputSystem())
            //.AddSystem(new GravitySystem())
            .AddSystem(new MovementSystem())
            .AddSystem(new CameraSystem(_camera))
            .AddSystem(new RenderSystem(SpriteBatch, _camera))
            .Build();

        CreateEcsEntities();
    }

    public override void Update(GameTime gameTime)
    {
        _ecsWorld.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_gameWorld.CurrentGameLevel.Tilemap.BackgroundColor ?? Color.Black);

        _renderer.Draw(SpriteBatch, _camera);
        _ecsWorld.Draw(gameTime);
    }

    private void CreateEcsEntities()
    {
        var texture = new Texture2D(GraphicsDevice, 16, 16);
        var data = new Color[16 * 16];
        for (var i = 0; i < data.Length; i++)
            data[i] = Color.White;
        texture.SetData(data);

        var player = _ecsWorld.CreateEntity();
        player.Attach(new Transform2(_gameWorld.CurrentGameLevel.Data.SpawnPoint));
        player.Attach(new Velocity());
        player.Attach(new Sprite(texture));
        player.Attach(new CameraTarget());
    }
}