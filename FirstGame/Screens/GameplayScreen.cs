using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Core;
using FirstGame.Collisions;
using FirstGame.Ecs.Components;
using FirstGame.Ecs.Systems;
using FirstGame.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MonoGame.Extended.ViewportAdapters;

namespace FirstGame.Screens;

public class GameplayScreen : BaseScreen
{
    private readonly TilemapSpriteBatchRenderer _renderer = new();
    private readonly CollisionWorld2D _collisionWorld = new();
    private OrthographicCamera _camera = null!;
    private GameWorld _gameWorld = null!;
    private World _ecsWorld = null!;

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
            .AddSystem(new GravitySystem())
            .AddSystem(new MovementSystem())
            .AddSystem(new CollisionSystem(_collisionWorld))
            .AddSystem(new CameraSystem(_camera))
            .AddSystem(new RenderSystem(SpriteBatch, _camera))
            .Build();

        CreateEcsEntities();
        CreateCollisions();
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

    private void CreateCollisions()
    {
        var tilemap = _gameWorld.CurrentGameLevel.Tilemap;
        if (tilemap.Layers["Walls"] is not TilemapTileLayer layer) return;

        _collisionWorld.AddLayer("Walls", new Layer(new SpatialHash(tilemap.TileToWorldPosition(layer.Width, layer.Height))){IsDynamic = false});

        var tiles = layer.GetTiles().ToArray();

        for (var i = 0; i < layer.GetTiles().Count(); i++)
        {
            var tile = tiles[i];
            var tilePosition = tilemap.TileToWorldPosition(tile.X, tile.Y);

            _collisionWorld.Insert(new TileActor(i, tilePosition.X, tilePosition.Y, layer.TileWidth, layer.TileHeight),
                "Walls");
        }

        // World border
        {
            const int border = 16;
            var width = layer.Width * layer.TileWidth;
            var height = layer.Height * layer.TileHeight;

            _collisionWorld.Insert(
                new BorderActor(
                    0,
                    new BoundingBox2D(
                        new Vector2(0, -border),
                        new Vector2(width, 0))),
                "Walls");

            _collisionWorld.Insert(
                new BorderActor(
                    1,
                    new BoundingBox2D(
                        new Vector2(0, height),
                        new Vector2(width, height + border))),
                "Walls");

            _collisionWorld.Insert(
                new BorderActor(
                    2,
                    new BoundingBox2D(
                        new Vector2(-border, 0),
                        new Vector2(0, height))),
                "Walls");

            _collisionWorld.Insert(
                new BorderActor(
                    3,
                    new BoundingBox2D(
                        new Vector2(width, 0),
                        new Vector2(width + border, height))),
                "Walls");        }
    }
}