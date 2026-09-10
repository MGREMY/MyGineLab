using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

public class PhysicsSystem : EntityUpdateSystem
{
    private readonly LevelProperties _properties;

    private ComponentMapper<VelocityComponent> _velocityComponentMapper = null!;
    private ComponentMapper<Transform2Component> _transformComponentMapper = null!;

    public PhysicsSystem(LevelProperties properties) : base(Aspect.All(typeof(VelocityComponent),
        typeof(Transform2Component)))
    {
        _properties = properties;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _velocityComponentMapper = mapperService.GetMapper<VelocityComponent>();
        _transformComponentMapper = mapperService.GetMapper<Transform2Component>();
    }

    public override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entityId in ActiveEntities)
        {
            var velocityComponent = _velocityComponentMapper.Get(entityId);
            var transformComponent = _transformComponentMapper.Get(entityId);

            // Apply gravity
            velocityComponent.Value.Y += _properties.GravityScale * _properties.Gravity * deltaTime;

            // Clamp the maximum fall speed
            if (velocityComponent.Value.Y > 0.0f)
            {
                velocityComponent.Value.Y = Math.Min(velocityComponent.Value.Y, _properties.MaxFallSpeed);
            }

            // Apply the velocity on the position
            transformComponent.Position += velocityComponent.Value * deltaTime;
        }
    }
}