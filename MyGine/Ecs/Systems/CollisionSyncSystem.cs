using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MyGine.Ecs.Components;

namespace MyGine.Ecs.Systems;

public sealed class CollisionSyncSystem2D : EntityUpdateSystem
{
    private ComponentMapper<Collider2D> _colliderMapper = null!;
    private ComponentMapper<Transform2> _transformMapper = null!;

    public CollisionSyncSystem2D()
        : base(Aspect.All(typeof(Collider2D), typeof(Transform2)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _colliderMapper = mapperService.GetMapper<Collider2D>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var transform = _transformMapper.Get(entityId);
            var collider = _colliderMapper.Get(entityId);

            collider.UpdatePosition(transform.Position);
        }
    }
}