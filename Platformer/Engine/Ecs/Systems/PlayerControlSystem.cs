using System;
using System.Reflection.Metadata;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

public class PlayerControlSystem : EntityUpdateSystem
{
    private readonly LevelProperties _properties;

    private ComponentMapper<PlayerComponent> _playerComponentMapper = null!;
    private ComponentMapper<VelocityComponent> _velocityComponentMapper = null!;

    public PlayerControlSystem(LevelProperties properties) : base(Aspect.All(typeof(PlayerComponent),
        typeof(VelocityComponent)))
    {
        _properties = properties;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _playerComponentMapper = mapperService.GetMapper<PlayerComponent>();
        _velocityComponentMapper = mapperService.GetMapper<VelocityComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entityId in ActiveEntities)
        {
            var playerComponent = _playerComponentMapper.Get(entityId);
            var velocityComponent = _velocityComponentMapper.Get(entityId);

            if (InputManager.Instance.Keyboard.IsKeyDown(Keys.Q))
            {
                velocityComponent.Target.X = Math.Clamp(velocityComponent.Target.X - _properties.WalkSpeed,
                    -_properties.WalkSpeed, _properties.WalkSpeed);
            }
            else if (velocityComponent.Target.X < 0) velocityComponent.Target.X = 0;

            if (InputManager.Instance.Keyboard.IsKeyDown(Keys.D))
            {
                velocityComponent.Target.X = Math.Clamp(velocityComponent.Target.X + _properties.WalkSpeed,
                    -_properties.WalkSpeed, _properties.WalkSpeed);
            }
            else if (velocityComponent.Target.X > 0) velocityComponent.Target.X = 0;

            if (playerComponent.CanJump
                && InputManager.Instance.Keyboard.WasKeyJustPressed(Keys.Space))
            {
                velocityComponent.Value.Y = -_properties.JumpSpeed;

                if (playerComponent.IsJumping) playerComponent.CanJump = false;

                playerComponent.IsJumping = true;
            }
        }
    }
}