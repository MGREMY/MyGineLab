using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

public class MovementSystem : EntityUpdateSystem
{
    private ComponentMapper<VelocityComponent> _velocityComponentMapper;

    public MovementSystem() : base(Aspect.All(typeof(VelocityComponent)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _velocityComponentMapper = mapperService.GetMapper<VelocityComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entityId in ActiveEntities)
        {
            var velocityComponent = _velocityComponentMapper.Get(entityId);

            velocityComponent.Value.X = MoveTowards(
                current: velocityComponent.Value.X,
                target: velocityComponent.Target.X,
                amount: Constants.MovementSpeed * deltaTime);

            /*
            velocityComponent.Value = MoveTowards(
                current: velocityComponent.Value,
                target: velocityComponent.Target,
                amount: Constants.MovementSpeed * deltaTime);
            */

            if (Math.Abs(velocityComponent.Value.X) <= 1f)
                velocityComponent.Value.X = 0.0f;

            /*
            if (Math.Abs(velocityComponent.Value.Y) <= 1f)
                velocityComponent.Value.Y = 0.0f;
            */
        }
    }

    /*
    private Vector2 MoveTowards(Vector2 current, Vector2 target, float amount) =>
        new(MoveTowards(current.X, target.X, amount), MoveTowards(current.Y, target.Y, amount));
    */

    private float MoveTowards(float current, float target, float amount)
    {
        if (Math.Abs(target - current) <= amount) return target;

        return current + Math.Sign(target - current) * amount;
    }
}