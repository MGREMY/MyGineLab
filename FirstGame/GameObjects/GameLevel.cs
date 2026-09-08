using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FirstGame.Collisions;
using FirstGame.Ecs.Components;
using FirstGame.Ecs.Systems;
using FirstGame.Helper;
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

namespace FirstGame.GameObjects;

public struct GameLevelData
{
    public Point VirtualCameraSize;
    public Vector2 SpawnPoint;
}

public class GameLevel
{
    public Tilemap Tilemap { get; }
    public GameLevelData Data { get; }

    [MemberNotNullWhen(true,
        nameof(_renderer),
        nameof(_camera),
        nameof(_collisions),
        nameof(_ecs))]
    private bool Initialized { get; set; }

    private TilemapSpriteBatchRenderer? _renderer;
    private OrthographicCamera? _camera;
    private CollisionWorld2D? _collisions;
    private World? _ecs;

    public GameLevel(Tilemap tilemap)
    {
        Tilemap = tilemap;
        Data = CreateData(tilemap);
    }

    [MemberNotNull(
        nameof(_renderer),
        nameof(_camera),
        nameof(_collisions),
        nameof(_ecs))]
    private void EnsureInitialized()
    {
        if (!Initialized) throw new InvalidOperationException("GameLevel must be initialized first.");
    }

    public void Initialize(GameWindow window, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _renderer = CreateRenderer(Tilemap);
        _camera = CreateCamera(window, graphicsDevice);
        _collisions = CreateCollisions(Tilemap);
        _ecs = CreateEcs(spriteBatch, _camera, _collisions);

        InitializeEntities(_ecs, TextureHelper.CreateFromSize(graphicsDevice, new Point(16), Color.White), Data);

        Initialized = true;
    }

    private GameLevelData CreateData(Tilemap tilemap)
    {
        const string dataLayerName = "MapData";

        var data = new GameLevelData();

        data.VirtualCameraSize.X = tilemap.Properties["VirtualCameraWidth"].AsInt();
        data.VirtualCameraSize.Y = tilemap.Properties["VirtualCameraHeight"].AsInt();

        if (tilemap.Layers[dataLayerName] is not TilemapObjectLayer dataLayer) return data;

        foreach (var o in dataLayer.Objects)
        {
            switch (o.Name)
            {
                case "SpawnPoint":
                {
                    data.SpawnPoint = o.Position;
                    break;
                }
            }
        }

        return data;
    }

    private TilemapSpriteBatchRenderer CreateRenderer(Tilemap tilemap)
    {
        var renderer = new TilemapSpriteBatchRenderer();
        renderer.LoadTilemap(tilemap);

        return renderer;
    }

    private OrthographicCamera CreateCamera(GameWindow window, GraphicsDevice graphicsDevice)
    {
        var viewportAdapter = new BoxingViewportAdapter(
            window: window,
            graphicsDevice: graphicsDevice,
            virtualWidth: Data.VirtualCameraSize.X,
            virtualHeight: Data.VirtualCameraSize.Y);
        var camera = new OrthographicCamera(viewportAdapter);

        return camera;
    }

    private CollisionWorld2D CreateCollisions(Tilemap tilemap)
    {
        const string layerName = "Walls";
        var idCounter = 0;

        var tilemapSize = tilemap.TileToWorldPosition(tilemap.Width, tilemap.Height);
        var collisionBroadphase = new SpatialHash(new SizeF(tilemapSize.X, tilemapSize.Y));
        var defaultLayer = new Layer(collisionBroadphase)
        {
            IsDynamic = false,
        };
        var collisions = new CollisionWorld2D(defaultLayer);

        if (tilemap.Layers[layerName] is not TilemapTileLayer tileLayer) return collisions;

        var tiles = tileLayer.GetTiles().ToArray();

        for (var i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            var tilePosition = tilemap.TileToWorldPosition(tile.X, tile.Y);

            var collisionShape = CollisionHelper.CreateFromPositionAndSize(
                x: tilePosition.X,
                y: tilePosition.Y,
                width: tilemap.TileWidth,
                height: tilemap.TileHeight);

            collisions.Insert(new TileActor(
                id: i + idCounter,
                collisionShape));
        }

        idCounter += tiles.Length;

        // World borders
        {
            // Top
            collisions.Insert(new BorderActor(
                id: ++idCounter,
                collisionShape: CollisionHelper.CreateFromPositionAndSize(
                    x: 0,
                    y: -tilemap.TileWidth,
                    width: tilemap.TileWidth,
                    height: 0)));
            // Bottom
            collisions.Insert(new BorderActor(
                id: ++idCounter,
                collisionShape: CollisionHelper.CreateFromPositionAndSize(
                    x: 0,
                    y: tilemap.Height,
                    width: tilemap.TileWidth,
                    height: tilemap.TileHeight)));
            // Left
            collisions.Insert(new BorderActor(
                id: ++idCounter,
                collisionShape: CollisionHelper.CreateFromPositionAndSize(
                    x: -tilemap.TileWidth,
                    y: 0,
                    width: tilemap.TileWidth,
                    height: tilemap.Height)));
            // Right
            collisions.Insert(new BorderActor(
                id: ++idCounter,
                collisionShape: CollisionHelper.CreateFromPositionAndSize(
                    x: tilemap.Width,
                    y: 0,
                    width: tilemap.TileWidth,
                    height: tilemap.Height)));
        }

        return collisions;
    }

    private World CreateEcs(SpriteBatch spriteBatch, OrthographicCamera camera, CollisionWorld2D collisions)
    {
        var ecs = new WorldBuilder()
            .AddSystem(new InputSystem())
            .AddSystem(new GravitySystem())
            .AddSystem(new MovementSystem())
            .AddSystem(new CollisionSystem(collisions))
            .AddSystem(new CameraSystem(camera))
            .AddSystem(new RenderSystem(spriteBatch, camera))
            .Build();

        return ecs;
    }

    private void InitializeEntities(World ecs, Texture2D defaultTexture, GameLevelData data)
    {
        var player = ecs.CreateEntity();
        player.Attach(new Transform2(data.SpawnPoint));
        player.Attach(new Velocity());
        player.Attach(new Sprite(defaultTexture));
        player.Attach(new CameraTarget());
    }

    public void Update(GameTime gameTime)
    {
        EnsureInitialized();

        _ecs.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        EnsureInitialized();

        _renderer.Draw(spriteBatch, _camera);
        _ecs.Draw(gameTime);
    }
}