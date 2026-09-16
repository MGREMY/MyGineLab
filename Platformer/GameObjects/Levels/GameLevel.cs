using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tilemaps;
using Platformer.Engine.Ecs.Components;
using Platformer.Engine.Ecs.Systems;
using Platformer.Extensions;

namespace Platformer.GameObjects.Levels;

public class GameLevel
{
    public readonly LevelProperties LevelProperties;

    private int _entityCount;
    private readonly CollisionWorld2D _collisions;
    private readonly World _ecs;

    public GameLevel(
        Tilemap map,
        OrthographicCamera camera,
        SpriteBatch spriteBatch)
    {
        LevelProperties = new LevelProperties(map);
        _collisions = new CollisionWorld2D();
        _ecs = new WorldBuilder()
            .AddSystem(new PlayerControlSystem(LevelProperties.Options))
            .AddSystem(new MovementSystem(LevelProperties.Options))
            .AddSystem(new PhysicsSystem(LevelProperties.Options))
            .AddSystem(new CollisionSyncSystem())
            .AddSystem(new CollisionQuerySystem(_collisions))
            .AddSystem(new CollisionResolutionSystem(LevelProperties.Options))
            .AddSystem(new CameraSystem(camera))
            .AddSystem(new SpriteSystem())
            .AddSystem(new RenderSystem(spriteBatch, camera))
            .Build();

        var box = new SpatialHash(map.TileToWorldPosition(map.Width, map.Height));
        _collisions.AddLayer(Constants.LevelCollisionsLayerWallName, new Layer(box) { IsDynamic = false });
        _collisions.AddLayer(Constants.LevelCollisionsLayerPlayerName, new Layer(box) { IsDynamic = true });
        _collisions.DisableSelfCollisions(Constants.LevelCollisionsLayerWallName);
        _collisions.DisableSelfCollisions(Constants.LevelCollisionsLayerPlayerName);
        _collisions.EnableCollisionBetweenLayers(
            Constants.LevelCollisionsLayerWallName,
            Constants.LevelCollisionsLayerPlayerName);

        if (map.Layers.GetLayer<TilemapTileLayer>(Constants.LDtkLayerWallName) is { } wallLayer)
        {
            foreach (var (x, y, width, height) in wallLayer.GetTileMerged(wallLayer.GetTileGrid()))
            {
                CreateAndInsertColliderComponent(
                    pos: map.TileToWorldPosition(x, y).ToVector2(),
                    size: map.TileToWorldPosition(width, height).ToVector2(),
                    Constants.LevelCollisionsLayerWallName);
            }
        }
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

    public void AddPlayer(Vector2 position, Sprite sprite)
    {
        var player = _ecs.CreateEntity();

        player.Attach(new PlayerComponent());
        player.Attach(new CameraTargetComponent());
        player.Attach(new Transform2Component { Position = position, Size = LevelProperties.Options.PlayerSize });
        player.Attach(new VelocityComponent());
        player.Attach(CreateAndInsertColliderComponent(
            position,
            LevelProperties.Options.PlayerSize,
            Constants.LevelCollisionsLayerPlayerName));
        player.Attach(new SpriteComponent(sprite));
    }

    public void Update(GameTime gameTime)
    {
        _ecs.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        _ecs.Draw(gameTime);
    }
}