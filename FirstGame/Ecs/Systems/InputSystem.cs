using System;
using Core.Input;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace FirstGame.Ecs.Systems;

public class InputSystem() : EntityUpdateSystem(Aspect.All(typeof(Velocity)))
{
    private const float Speed = 300f; // 300 PX/s

    private readonly Vector2 _speedVector = new(Speed);
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
            var newVelocityTarget = velocity.Target;

            if (InputManager.Instance.Keyboard.IsKeyDown(Keys.Q)) newVelocityTarget.X = Math.Clamp(newVelocityTarget.X - _speedVector.X, -Speed, Speed);
            else if (newVelocityTarget.X < 0) newVelocityTarget.X = 0;
            if (InputManager.Instance.Keyboard.IsKeyDown(Keys.D)) newVelocityTarget.X = Math.Clamp(newVelocityTarget.X + _speedVector.X, -Speed, Speed);
            else if (newVelocityTarget.X > 0) newVelocityTarget.X = 0;

            if (InputManager.Instance.Keyboard.IsKeyDown(Keys.Space))
            {
                if (velocity.CanGoUp)
                {
                    velocity.CanGoUp = false;
                    velocity.Value.Y -= _speedVector.Y;
                }
            }

            velocity.Target = newVelocityTarget;
        }
    }
}