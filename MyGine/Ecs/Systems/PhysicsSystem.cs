using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MyGine.Ecs.Components;

namespace MyGine.Ecs.Systems;

public sealed class PhysicsSystem2D : EntityUpdateSystem
{
    private readonly SceneServices _services;

    private ComponentMapper<RigidBody2D> _rigidBodyMapper = null!;
    private ComponentMapper<Transform2> _transformMapper = null!;

    public PhysicsSystem2D(SceneServices services)
        : base(Aspect
            .All(typeof(Transform2))
            .One(typeof(RigidBody2D)))
    {
        _services = services;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _rigidBodyMapper = mapperService.GetMapper<RigidBody2D>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entityId in ActiveEntities)
        {
            var rigidBody = _rigidBodyMapper.Get(entityId);
            var transform = _transformMapper.Get(entityId);

            var gravity = rigidBody.Mass * rigidBody.GravityScale * _services.App.Settings.GravityAcceleration * dt;
            var newVelocity = rigidBody.Velocity + new Vector2(0, gravity);

            if (newVelocity.Y < 0.0f)
            {
                newVelocity.Y = MathF.Max(newVelocity.Y, _services.App.Settings.TerminalVelocity);
            }

            rigidBody.Velocity = newVelocity;
            transform.Position += rigidBody.Velocity * dt;
        }
    }
}

public sealed class PhysicsSystem3D : EntityUpdateSystem
{
    public PhysicsSystem3D()
        : base(Aspect.All(typeof(Transform3)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        throw new NotImplementedException();
    }

    public override void Update(GameTime gameTime)
    {
        throw new NotImplementedException();
    }
}