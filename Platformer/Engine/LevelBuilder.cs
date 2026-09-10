using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using Platformer.Engine.Collisions;
using Platformer.Engine.Ecs.Components;
using Platformer.Engine.Helpers;

namespace Platformer.Engine;

public static class LevelBuilder
{
    private static int _nextId;

    public static LevelData Build(
        string levelPath,
        ContentManager content,
        LevelProperties properties,
        World ecs,
        CollisionWorld2D collisions)
    {
        var tilemap = content.Load<Tilemap>(levelPath);

        return Build(tilemap, content, properties, ecs, collisions);
    }

    public static LevelData Build(
        Tilemap tilemap,
        ContentManager content,
        LevelProperties properties,
        World ecs,
        CollisionWorld2D collisions)
    {
        var wallActors = CreateCollisions(tilemap, properties).Select(x => new WallActor(_nextId++, x)).ToArray();
        var spawn = CreateSpawnPoint(tilemap, properties);
        var player = CreatePlayer(content, properties, ecs, collisions, spawn);
        var entities = CreateEntities(tilemap, properties, ecs, collisions);

        var renderer = new TilemapSpriteBatchRenderer
        {
            BlendState = BlendState.AlphaBlend,
        };
        renderer.LoadTilemap(tilemap);

        foreach (var wall in wallActors) collisions.Insert(wall, properties.CollisionWallLayer);

        return new LevelData
        {
            Tilemap = tilemap,
            Renderer = renderer,
            Walls = wallActors,
            EntityIds = [player.Id, .. entities.Select(x => x.Id)],
            SpawnPoint = spawn,
            SizePx = tilemap.TileToWorldPosition(tilemap.Width, tilemap.Height),
        };
    }

    public static LevelProperties LoadProperties(Tilemap map)
    {
        var properties = new LevelProperties
        {
            VirtualCameraSize = new Point(
                map.Properties.GetInt("VirtualCameraWidth", Constants.CameraWidth),
                map.Properties.GetInt("VirtualCameraHeight", Constants.CameraHeight)),
            PlayerWidth = map.Properties.GetFloat("PlayerWidth", Constants.PlayerWidth),
            PlayerHeight = map.Properties.GetFloat("PlayerHeight", Constants.PlayerHeight),
            Gravity = map.Properties.GetFloat("Gravity", Constants.Gravity),
            GravityScale = map.Properties.GetFloat("GravityScale", Constants.GravityScale),
            MoveAcceleration = map.Properties.GetFloat("MoveAcceleration", Constants.MoveAcceleration),
            WalkSpeed = map.Properties.GetFloat("WalkSpeed", Constants.WalkSpeed),
            JumpSpeed = map.Properties.GetFloat("JumpSpeed", Constants.JumpSpeed),
            WallSlideSpeed = map.Properties.GetFloat("WallSlideSpeed", Constants.WallSlideSpeed),
            MaxFallSpeed = map.Properties.GetFloat("MaxFallSpeed", Constants.MaxFallSpeed),
        };

        return properties;
    }

    private static IEnumerable<CollisionShape2D> CreateCollisions(Tilemap map, LevelProperties properties)
    {
        if (map.Layers[properties.TilemapWallLayer] is not TilemapTileLayer tileLayer) return [];

        var grid = new int[tileLayer.Width, tileLayer.Height];
        foreach (var entry in tileLayer.GetTiles())
            grid[entry.X, entry.Y] = 1;

        return MergeSolidRuns(grid)
            .Select(r => new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(
                map.TileToWorldPosition(r.X, r.Y).ToVector2(),
                map.TileToWorldPosition(r.Width, r.Height).ToVector2()
            )));
    }

    /// Sweeps the grid row by row and collapses contiguous solid tiles into
    /// axis-aligned rectangles that span as many rows as possible.
    ///
    /// Idea: for each horizontal run of solid tiles in a row, remember it as
    /// (start, end). If the *same* run also appears in the next row, it's not a
    /// new rectangle — the existing one just got one row taller. Only when a
    /// run disappears do we know its height and emit the finished rectangle.
    /// A final extra (empty) pass forces every still-open rectangle to flush.
    private static IEnumerable<Rectangle> MergeSolidRuns(int[,] solid)
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
                    yield return new Rectangle(kvp.Key.start, kvp.Value, kvp.Key.end - kvp.Key.start, y - kvp.Value);

            active = next;
        }
    }

    private static Vector2 CreateSpawnPoint(Tilemap map, LevelProperties properties)
    {
        if (map.Layers[properties.TilemapDataLayer] is TilemapObjectLayer dataLayer)
        {
            foreach (var o in dataLayer.Objects)
            {
                if (o.Name == properties.TilemapDataDefinitionSpawnPoint)
                {
                    return o.Position;
                }
            }
        }

        return Vector2.Zero;
    }

    private static Entity CreatePlayer(
        ContentManager content,
        LevelProperties properties,
        World ecs,
        CollisionWorld2D collisions,
        Vector2 spawnPoint)
    {
        var playerSize = new Vector2(properties.PlayerWidth, properties.PlayerHeight);

        var player = ecs.CreateEntity();
        player.Attach(new PlayerComponent());
        player.Attach(new CameraTargetComponent());
        player.Attach(new Transform2Component { Position = spawnPoint, Size = playerSize });
        player.Attach(new VelocityComponent());
        player.Attach(CreateCollider(collisions, spawnPoint, playerSize, properties.CollisionPlayerLayer));
        player.Attach(
            new SpriteComponent(
                SpriteHelper.CreateAnimatedSprite(content, Path.Combine("texture", "player", "player"))));

        return player;
    }

    private static IEnumerable<Entity> CreateEntities(
        Tilemap map,
        LevelProperties properties,
        World ecs,
        CollisionWorld2D collisions)
    {
        return [];
    }

    private static ColliderComponent CreateCollider(
        CollisionWorld2D collisions,
        Vector2 pos,
        Vector2 size,
        string layer)
    {
        var collider = new ColliderComponent(
            id: _nextId++,
            shape: new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(pos, size)),
            layerName: layer);

        collisions.Insert(collider, layer);

        return collider;
    }
}