using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

/// <summary>
/// Queries the collision world for every collider and stores the collisions it
/// found on this frame, so other systems can resolve them.
/// </summary>
public class CollisionQuerySystem : EntityUpdateSystem
{
    private readonly LevelProperties _properties;
    private readonly CollisionWorld2D _collisions;

    private ComponentMapper<ColliderComponent> _colliderComponentMapper = null!;

    /// <summary>
    /// Initializes a new instance of the collision query system.
    /// </summary>
    /// <param name="properties">The properties of the level.</param>
    /// <param name="collisions">The collision world to query.</param>
    public CollisionQuerySystem(LevelProperties properties, CollisionWorld2D collisions) : base(
        Aspect.All(typeof(ColliderComponent)))
    {
        _properties = properties;
        _collisions = collisions;
    }

    /// <inheritdoc/>
    public override void Initialize(IComponentMapperService mapperService)
    {
        _colliderComponentMapper = mapperService.GetMapper<ColliderComponent>();
    }

    /// <summary>
    /// Rebuilds the dynamic collision layers and queries the collisions of every
    /// collider against the level's wall layer.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Update(GameTime gameTime)
    {
        _collisions.RebuildDynamicLayers();

        foreach (var entityId in ActiveEntities)
        {
            var colliderComponent = _colliderComponentMapper.Get(entityId);

            var collisions = _collisions.QueryCollisions(colliderComponent, _properties.CollisionWallLayer);
            colliderComponent.LastFrameCollisions = [.. collisions];
        }
    }
}