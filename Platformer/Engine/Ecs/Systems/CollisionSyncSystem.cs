using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

/// <summary>
/// Keeps every collider's shape aligned with its entity's transform by moving
/// the collider so its top-left corner matches the transform's position.
/// </summary>
public class CollisionSyncSystem : EntityUpdateSystem
{
    private ComponentMapper<Transform2Component> _transformComponentMapper = null!;
    private ComponentMapper<ColliderComponent> _colliderComponentMapper = null!;

    /// <summary>
    /// Initializes a new instance of the collision sync system.
    /// </summary>
    public CollisionSyncSystem() : base(Aspect.All(typeof(Transform2Component), typeof(ColliderComponent)))
    {
    }

    /// <inheritdoc/>
    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformComponentMapper = mapperService.GetMapper<Transform2Component>();
        _colliderComponentMapper = mapperService.GetMapper<ColliderComponent>();
    }

    /// <summary>
    /// Aligns each collider with its entity's transform.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var transformComponent = _transformComponentMapper.Get(entityId);
            var colliderComponent = _colliderComponentMapper.Get(entityId);

            colliderComponent.UpdatePosition(transformComponent.Position);
        }
    }
}