using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MyGine.Ecs.Components;

namespace MyGine.Ecs.Systems;

public sealed class CollisionQuerySystem2D : EntityUpdateSystem
{
    private readonly SceneServices _services;

    private ComponentMapper<Collider2D> _colliderMapper = null!;

    public CollisionQuerySystem2D(SceneServices services)
        : base(Aspect.All(typeof(Collider2D)))
    {
        _services = services;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _colliderMapper = mapperService.GetMapper<Collider2D>();
    }

    public override void Update(GameTime gameTime)
    {
        if (_services.CollisionWorld is null) return;
        
        _services.CollisionWorld.RebuildDynamicLayers();

        foreach (var entityId in ActiveEntities)
        {
            var collider = _colliderMapper.Get(entityId);
            collider.Collisions.Clear();

            foreach (var targetLayer in collider.Queries)
            {
                if (!_services.CollisionWorld.IsCollisionEnabledBetweenLayers(collider.Layer, targetLayer)) continue;

                foreach (var collision in _services.CollisionWorld.QueryCollisions(collider, targetLayer))
                {
                    collider.Collisions.Add(collision);
                }
            }
        }
    }
}