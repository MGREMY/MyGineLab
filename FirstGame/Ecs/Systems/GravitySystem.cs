using System;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace FirstGame.Ecs.Systems;

public class GravitySystem() : EntityUpdateSystem(Aspect.All(typeof(Velocity)))
{
    private const float GravityAcceleration = 100f; // 150 PX/s

    private ComponentMapper<Velocity> _velocityMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _velocityMapper = mapperService.GetMapper<Velocity>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var velocity = _velocityMapper.Get(entityId);

            if (velocity.Target.Y < GravityAcceleration * 4) velocity.Target.Y += GravityAcceleration;
        }
    }
}