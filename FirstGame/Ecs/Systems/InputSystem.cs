using System;
using Core.Input;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace FirstGame.Ecs.Systems;

public class InputSystem() : EntityUpdateSystem(Aspect.All(typeof(Control)))
{
    private const float Speed = 300f; // 300 PX/s

    private readonly Vector2 _speedVector = new(Speed);

    private ComponentMapper<Velocity> _velocityMapper = null!;
    private ComponentMapper<Control> _controllableMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _velocityMapper = mapperService.GetMapper<Velocity>();
        _controllableMapper = mapperService.GetMapper<Control>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var controllable = _controllableMapper.Get(entityId);

            // If the entity doesn't have the Velocity component, skip this section
            if (_velocityMapper.TryGet(entityId, out var velocity))
            {
                var nextVelocity = velocity.Target;

                // For the X axis, we first check if the user wants to go in one direction
                // If so, we add the max speed
                // Otherwise if the user was going in this direction, we reset the velocity target
                //
                // We only set the velocity to 0 if the user was going towards this direction
                // This avoid overriding the previously set velocity target
                if (InputManager.Instance.Keyboard.IsKeyDown(Keys.Q))
                    nextVelocity.X = Math.Clamp(nextVelocity.X - Speed, -Speed, Speed);
                else if (nextVelocity.X < 0) nextVelocity.X = 0;
                if (InputManager.Instance.Keyboard.IsKeyDown(Keys.D))
                    nextVelocity.X = Math.Clamp(nextVelocity.X + Speed, -Speed, Speed);
                else if (nextVelocity.X > 0) nextVelocity.X = 0;

                // For the Y axis, we verify if the user can jump
                // If he can we proceed, otherwise we skip
                //
                // We check if the user has just pressed the jump button
                //
                // Then if the user can jump and ask to jump, we jump
                // We check if the user was already jumping
                // If not, we set this variable to true
                // Otherwise, we set CanJump to false, this prevents the user from double jumping more than one time
                if (controllable.CanJump
                    && InputManager.Instance.Keyboard.WasKeyJustPressed(Keys.Space))
                {
                    velocity.Value.Y = -Speed;

                    if (controllable.IsJumping) controllable.CanJump = false;

                    controllable.IsJumping = true;
                }

                velocity.Target = nextVelocity;
            }
        }
    }
}