using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MyGine.Ecs.Components;

namespace MyGine.Ecs.Systems;

public sealed class CollisionResolutionSystem2D : EntityUpdateSystem
{
    private ComponentMapper<Collider2D> _colliderMapper = null!;
    private ComponentMapper<Transform2> _transformMapper = null!;
    private ComponentMapper<RigidBody2D> _rigidBodyMapper = null!;

    public CollisionResolutionSystem2D()
        : base(Aspect.All(typeof(Collider2D), typeof(Transform2)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _colliderMapper = mapperService.GetMapper<Collider2D>();
        _transformMapper = mapperService.GetMapper<Transform2>();
        _rigidBodyMapper = mapperService.GetMapper<RigidBody2D>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var collider = _colliderMapper.Get(entityId);

            if (collider.Mtv == Vector2.Zero || collider.CollisionResponse is CollisionResponse.None)
                continue;

            var transform = _transformMapper.Get(entityId);
            transform.Position += collider.Mtv;

            if (collider.CollisionResponse is CollisionResponse.PushOutAndStop
                && _rigidBodyMapper.TryGet(entityId, out var rigidBody))
            {
                var keep = new Vector2(collider.Mtv.X == 0.0f ? 1f : 0f, collider.Mtv.Y == 0.0f ? 1f : 0f);
                rigidBody.Velocity *= keep;
            }
        }
    }
}