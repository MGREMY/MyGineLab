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

/// <summary>
/// Represents a playable level: it builds its own ECS world, collision world,
/// entities and camera, then drives them each frame.
/// </summary>
public class GameLevel
{
    /// <summary>
    /// Gets the tilemap describing the level geometry.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of a level from the given tilemap.
    /// </summary>
    /// <param name="tilemap">The tilemap describing the level geometry.</param>
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

    /// <summary>
    /// Initializes the level: loads its properties, creates the camera, the
    /// collision world, the ECS world with all its systems, and the level
    /// entities.
    /// </summary>
    /// <param name="window">The game window used by the camera viewport adapter.</param>
    /// <param name="graphicsDevice">The graphics device used to create rendering resources.</param>
    /// <param name="content">The content manager used to load level assets.</param>
    /// <param name="spriteBatch">The sprite batch used to render the level.</param>
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

    /// <summary>
    /// Updates the level by running every ECS system. Requires
    /// <see cref="Initialize"/> to have been called first.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public void Update(GameTime gameTime)
    {
        EnsureInitialized();

        _ecs.Update(gameTime);
    }

    /// <summary>
    /// Draws the level's tilemap and then every ECS draw system. Requires
    /// <see cref="Initialize"/> to have been called first.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    /// <param name="spriteBatch">The sprite batch used to render the level.</param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        EnsureInitialized();

        _data.Renderer.Draw(spriteBatch, _camera);
        _ecs.Draw(gameTime);
    }
}