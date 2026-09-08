using System;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace FirstGame.Ecs.Systems;

public class MovementSystem() : EntityUpdateSystem(Aspect.All(typeof(Transform2), typeof(Velocity)))
{
    private const float MovementSpeed = 1200f; // 600 PX/s

    private ComponentMapper<Transform2> _transformMapper = null!;
    private ComponentMapper<Velocity> _velocityMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<Velocity>();
    }

    public override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entityId in ActiveEntities)
        {
            var transform = _transformMapper.Get(entityId);
            var velocity = _velocityMapper.Get(entityId);

            velocity.Value = MoveTowards(velocity.Value, velocity.Target, MovementSpeed * deltaTime);
            transform.Position += velocity.Value * deltaTime;

            if (Math.Abs(velocity.Value.X) <= 1f) velocity.Target.X = 0;
            if (Math.Abs(velocity.Value.Y) <= 1f) velocity.Target.Y = 0;
        }
    }

    private Vector2 MoveTowards(Vector2 current, Vector2 target, float amount)
    {
        return new Vector2(MoveTowards(current.X, target.X, amount), MoveTowards(current.Y, target.Y, amount));
    }

    private float MoveTowards(float current, float target, float amount)
    {
        if (Math.Abs(target - current) <= amount) return target;

        return current + Math.Sign(target - current) * amount;
    }
}