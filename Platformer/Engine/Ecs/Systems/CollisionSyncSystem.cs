using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

public class CollisionSyncSystem : EntityUpdateSystem
{
    private ComponentMapper<Transform2Component> _transformComponentMapper = null!;
    private ComponentMapper<ColliderComponent> _colliderComponentMapper = null!;

    public CollisionSyncSystem() : base(Aspect.All(typeof(Transform2Component), typeof(ColliderComponent)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformComponentMapper = mapperService.GetMapper<Transform2Component>();
        _colliderComponentMapper = mapperService.GetMapper<ColliderComponent>();
    }

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