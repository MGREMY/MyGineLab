using System;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace FirstGame.Ecs.Systems;

public class GravitySystem() : EntityUpdateSystem(Aspect.All(typeof(Velocity)))
{
    private const float GravityAcceleration = 100f;

    private ComponentMapper<Velocity> _velocityMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _velocityMapper = mapperService.GetMapper<Velocity>();
    }

    public override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entityId in ActiveEntities)
        {
            var velocity = _velocityMapper.Get(entityId);

            // If velocity Y is less than -GravitySpeed, we skip this entity
            if (velocity.Value.Y <= -GravityAcceleration) continue;

            velocity.Value.Y -= -GravityAcceleration * deltaTime;
        }
    }
}