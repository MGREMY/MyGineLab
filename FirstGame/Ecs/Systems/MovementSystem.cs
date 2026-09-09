using System;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace FirstGame.Ecs.Systems;

public class NewMovementSystem(CollisionWorld2D collisionWorld)
    : EntityUpdateSystem(Aspect.All(typeof(Transform2), typeof(Velocity)))
{
    private const float MovementSpeed = 1200f;

    private ComponentMapper<Transform2> _transformMapper = null!;
    private ComponentMapper<Velocity> _velocityMapper = null!;
    private ComponentMapper<Collision> _collisionMapper = null!;
    private ComponentMapper<Control> _controlMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<Velocity>();
        _collisionMapper = mapperService.GetMapper<Collision>();
        _controlMapper = mapperService.GetMapper<Control>();
    }

    public override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entityId in ActiveEntities)
        {
            var transform = _transformMapper.Get(entityId);
            var velocity = _velocityMapper.Get(entityId);

            // Process collisions if there are any
            if (_collisionMapper.TryGet(entityId, out Collision? collision)
                && collision.Correction != Vector2.Zero)
            {
                // If there is a collision, reset jump status
                if (_controlMapper.TryGet(entityId, out var control))
                {
                    control.CanJump = true;
                    control.IsJumping = false;
                }

                // If some collision happen on X axis
                if (collision.Correction.X is not 0)
                {
                    // If velocity is going up, reset it
                    if (velocity.Value.Y < 0) velocity.Value.Y = 0;

                    // If we have a collision on one side and the user
                    // want to move towards it, We reduce falling
                    // intensity to simulate the slide on the wall
                    if ((velocity.Target.X > 0 && collision.Correction.X > 0)
                        || (velocity.Target.X < 0 && collision.Correction.X < 0))
                    {
                        // If the user is jumping
                        // we first need to move to the opposite side
                        if (velocity.Target.Y < 0)
                        {
                            velocity.Value.X = -velocity.Target.X;
                        }
                        // Otherwise, we reduce the falling velocity so simulate some wall grabbing
                        else
                        {
                            velocity.Value.X = 0;
                            velocity.Value.Y *= 0.75f;
                        }
                    }
                }

                if (collision.Correction.Y < 0)
                {
                    velocity.Value.Y = 0;
                }
            }

            velocity.Value = MoveTowards(velocity.Value, velocity.Target, MovementSpeed * deltaTime);
            if (Math.Abs(velocity.Value.X) <= 1f) velocity.Target.X = 0;
            if (Math.Abs(velocity.Value.Y) <= 1f) velocity.Target.Y = 0;

            transform.Position += velocity.Value * deltaTime;
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