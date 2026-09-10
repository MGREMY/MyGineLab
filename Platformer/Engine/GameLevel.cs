using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.ViewportAdapters;
using Platformer.Engine.Ecs.Systems;

namespace Platformer.Engine;

public class GameLevel
{
    public Tilemap Tilemap { get; }

    [MemberNotNullWhen(
        true,
        nameof(_camera),
        nameof(_ecs),
        nameof(_collisions),
        nameof(_data),
        nameof(_properties))]
    private bool Initialized { get; set; }

    private OrthographicCamera? _camera;
    private World? _ecs;
    private CollisionWorld2D? _collisions;
    private LevelData? _data;
    private LevelProperties? _properties;

    public GameLevel(Tilemap tilemap)
    {
        Tilemap = tilemap;
    }

    [MemberNotNull(
        nameof(_camera),
        nameof(_ecs),
        nameof(_collisions),
        nameof(_data),
        nameof(_properties))]
    private void EnsureInitialized()
    {
        if (!Initialized) throw new InvalidOperationException("GameLevel must be initialized first.");
    }

    public void Initialize(
        GameWindow window,
        GraphicsDevice graphicsDevice,
        ContentManager content,
        SpriteBatch spriteBatch)
    {
        _properties = LevelBuilder.LoadProperties(Tilemap);

        _camera = new OrthographicCamera(new BoxingViewportAdapter(
            window: window,
            graphicsDevice: graphicsDevice,
            virtualWidth: _properties.VirtualCameraSize.X,
            virtualHeight: _properties.VirtualCameraSize.Y));
        _camera.EnableWorldBounds(Tilemap.WorldBounds);
        _camera.IsZoomClampedToWorldBounds = true;

        var box = new SpatialHash(Tilemap.TileToWorldPosition(Tilemap.Width, Tilemap.Height));
        _collisions = new CollisionWorld2D();
        _collisions.AddLayer(Constants.WallLayer, new Layer(box) { IsDynamic = false });
        _collisions.AddLayer(Constants.PlayerLayer, new Layer(box) { IsDynamic = true });
        _collisions.DisableCollisionBetweenLayers(Constants.WallLayer, Constants.WallLayer);
        _collisions.DisableCollisionBetweenLayers(Constants.PlayerLayer, Constants.PlayerLayer);
        _collisions.EnableCollisionBetweenLayers(Constants.WallLayer, Constants.PlayerLayer);

        _ecs = new WorldBuilder()
            .AddSystem(new PlayerControlSystem(_properties))
            .AddSystem(new MovementSystem(_properties))
            .AddSystem(new PhysicsSystem(_properties))
            .AddSystem(new CollisionSyncSystem())
            .AddSystem(new CollisionQuerySystem(_properties, _collisions))
            .AddSystem(new CollisionResolutionSystem(_properties))
            .AddSystem(new CameraSystem(_camera))
            .AddSystem(new SpriteSystem())
            .AddSystem(new RenderSystem(_properties, spriteBatch, _camera))
            .Build();

        _data = LevelBuilder.Build(Tilemap, content, _properties, _ecs, _collisions);

        Initialized = true;
    }

    public void Update(GameTime gameTime)
    {
        EnsureInitialized();

        _ecs.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        EnsureInitialized();

        _data.Renderer.Draw(spriteBatch, _camera);
        _ecs.Draw(gameTime);
    }
}