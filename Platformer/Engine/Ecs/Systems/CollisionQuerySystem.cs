using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

public class CollisionQuerySystem : EntityUpdateSystem
{
    private readonly LevelProperties _properties;
    private readonly CollisionWorld2D _collisions;

    private ComponentMapper<ColliderComponent> _colliderComponentMapper = null!;

    public CollisionQuerySystem(LevelProperties properties, CollisionWorld2D collisions) : base(
        Aspect.All(typeof(ColliderComponent)))
    {
        _properties = properties;
        _collisions = collisions;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _colliderComponentMapper = mapperService.GetMapper<ColliderComponent>();
    }

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