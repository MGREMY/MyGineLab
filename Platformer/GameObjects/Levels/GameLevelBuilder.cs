using System;
using System.Collections.Generic;
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
using Platformer.Engine.Ecs.Components;
using Platformer.Engine.Ecs.Systems;

namespace Platformer.GameObjects.Levels;

public class GameLevelBuilder
{
    private readonly Tilemap _map;
    private readonly LevelProperties _properties;
    private readonly Sprite _playerSprite;
    private readonly OrthographicCamera _camera;
    private readonly CollisionWorld2D _collisions;
    private readonly World _ecs;
    private readonly TilemapSpriteBatchRenderer _renderer;

    private int _entityCount;

    public GameLevelBuilder(
        Tilemap map,
        LevelProperties properties,
        Sprite playerSprite,
        GameWindow window,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch)
    {
        _map = map;
        _properties = properties;
        _playerSprite = playerSprite;
        _renderer = new TilemapSpriteBatchRenderer
        {
            BlendState = BlendState.AlphaBlend,
        };
        _renderer.LoadTilemap(_map);

        _camera = new OrthographicCamera(new BoxingViewportAdapter(
            window: window,
            graphicsDevice: graphicsDevice,
            virtualWidth: _properties.VirtualCameraSize.X,
            virtualHeight: _properties.VirtualCameraSize.Y));
        _camera.EnableWorldBounds(_map.WorldBounds);
        _camera.IsZoomClampedToWorldBounds = true;

        var box = new SpatialHash(map.TileToWorldPosition(map.Width, map.Height));
        _collisions = new CollisionWorld2D();
        _collisions.AddLayer(LevelConstants.WallLayer, new Layer(box) { IsDynamic = false });
        _collisions.AddLayer(LevelConstants.PlayerLayer, new Layer(box) { IsDynamic = true });
        _collisions.DisableCollisionBetweenLayers(LevelConstants.WallLayer, LevelConstants.WallLayer);
        _collisions.DisableCollisionBetweenLayers(LevelConstants.PlayerLayer, LevelConstants.PlayerLayer);
        _collisions.EnableCollisionBetweenLayers(LevelConstants.WallLayer, LevelConstants.PlayerLayer);

        _ecs = new WorldBuilder()
            .AddSystem(new PlayerControlSystem(_properties))
            .AddSystem(new MovementSystem(_properties))
            .AddSystem(new PhysicsSystem(_properties))
            .AddSystem(new CollisionSyncSystem())
            .AddSystem(new CollisionQuerySystem(_collisions))
            .AddSystem(new CollisionResolutionSystem(_properties))
            .AddSystem(new CameraSystem(_camera))
            .AddSystem(new SpriteSystem())
            .AddSystem(new RenderSystem(spriteBatch, _camera))
            .Build();
    }

    public GameLevelBuilder WithPlayer()
    {
        if (GetSpawnPoint() is not { } spawnPoint) return this;

        var player = _ecs.CreateEntity();
        player.Attach(new PlayerComponent());
        player.Attach(new CameraTargetComponent());
        player.Attach(new Transform2Component { Position = spawnPoint, Size = _properties.PlayerSize });
        player.Attach(new VelocityComponent());
        player.Attach(CreateAndInsertColliderComponent(spawnPoint, _properties.PlayerSize, LevelConstants.PlayerLayer));
        player.Attach(new SpriteComponent(_playerSprite));

        return this;
    }

    public GameLevelBuilder WithEntities()
    {
        return this;
    }

    public GameLevelBuilder WithWalls()
    {
        if (_map.Layers[LayerConstants.WallLayer] is not TilemapTileLayer wallLayer) return this;

        var grid = new int[wallLayer.Width, wallLayer.Height];
        foreach (var entry in wallLayer.GetTiles())
            grid[entry.X, entry.Y] = 1;

        foreach (var (x, y, width, height) in MergeSolidRuns(grid))
        {
            CreateAndInsertColliderComponent(
                pos: _map.TileToWorldPosition(x, y).ToVector2(),
                size: _map.TileToWorldPosition(width, height).ToVector2(),
                LevelConstants.WallLayer);
        }

        return this;
    }

    private Vector2? GetSpawnPoint()
    {
        if (_map.Layers[LayerConstants.DataLayer] is not TilemapObjectLayer dataLayer) return null;

        foreach (var o in dataLayer.Objects)
        {
            if (o.Name is Constants.SpawnPoint)
            {
                return o.Position;
            }
        }

        return null;
    }

    private ColliderComponent CreateAndInsertColliderComponent(
        Vector2 pos,
        Vector2 size,
        string layer)
    {
        var collider = new ColliderComponent(
            id: _entityCount++,
            shape: new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(pos, size)),
            layerName: layer);

        _collisions.Insert(collider, layer);

        return collider;
    }

    /// <summary>
    /// Sweeps the grid row by row and collapses contiguous solid tiles into
    /// axis-aligned rectangles that span as many rows as possible.
    ///
    /// Idea: for each horizontal run of solid tiles in a row, remember it as
    /// (start, end). If the *same* run also appears in the next row, it's not a
    /// new rectangle — the existing one just got one row taller. Only when a
    /// run disappears do we know its height and emit the finished rectangle.
    /// A final extra (empty) pass forces every still-open rectangle to flush.
    /// </summary>
    private static IEnumerable<(int x, int y, int width, int height)> MergeSolidRuns(int[,] solid)
    {
        var width = solid.GetLength(0);
        var height = solid.GetLength(1);

        // active maps "horizontal run key (start, end)" -> the row where that
        // run first appeared (the top edge of the growing rectangle).
        var active = new Dictionary<(int start, int end), int>();

        // One pass per row, plus one sentinel pass (y == height) where the row
        // is empty on purpose. That final pass has "no run continues here", so
        // every remaining active rectangle is flushed.
        for (var y = 0; y <= height; y++)
        {
            var next = new Dictionary<(int, int), int>();

            // Build the horizontal runs of this row: consecutive solid cells
            // with the same "class" (int-grid value) form a single run.
            // Key is (start, x) with x exclusive — a half-open interval.
            if (y < height)
            {
                for (var x = 0; x < width;)
                {
                    if (solid[x, y] == 0)
                    {
                        x++;
                        continue;
                    }

                    var start = x;
                    var cls = solid[start, y];
                    // merge only equal class ids
                    while (x < width && solid[x, y] == cls) x++;

                    // candidate: run starts a rectangle at THIS row...
                    next[(start, x)] = y;
                }
            }

            // ...unless it's a continuation of a run from the row above. If the
            // same (start,end) existed in the active row, the rectangle did NOT
            // end here, so keep its original top edge instead of the current y.
            foreach (var kvp in next)
                if (active.TryGetValue(kvp.Key, out var yStart))
                    next[kvp.Key] = yStart;

            // Any run from the previous row that did NOT continue into this one
            // is a finished rectangle: top = its yStart, height = y - yStart
            // (y acts as the exclusive bottom edge). Emit and forget it.
            foreach (var kvp in active)
                if (!next.ContainsKey(kvp.Key))
                    yield return new ValueTuple<int, int, int, int>(
                        kvp.Key.start,
                        kvp.Value,
                        kvp.Key.end - kvp.Key.start,
                        y - kvp.Value);

            active = next;
        }
    }

    public GameLevel Build()
    {
        return new GameLevel(_map, _ecs, _collisions, _camera, _renderer);
    }
}