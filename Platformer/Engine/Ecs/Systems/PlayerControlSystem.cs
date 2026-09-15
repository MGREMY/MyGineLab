using System;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;
using Platformer.GameObjects.Levels;

namespace Platformer.Engine.Ecs.Systems;

/// <summary>
/// Reads the player's input and translates it into velocity targets and jumps.
/// </summary>
/// <remarks>
/// Movement uses a target velocity that <see cref="MovementSystem"/> ramps
/// toward: Q pushes left, D pushes right and releasing the key sets the target
/// back to zero. Jumping applies an immediate upward impulse only when the
/// player is allowed to jump; a player that is mid-air after jumping has the
/// jump allowance revoked on the first jump, enabling a single double jump.
/// </remarks>
public class PlayerControlSystem : EntityUpdateSystem
{
    private readonly LevelProperties _properties;

    private ComponentMapper<PlayerComponent> _playerComponentMapper = null!;
    private ComponentMapper<VelocityComponent> _velocityComponentMapper = null!;
    private ComponentMapper<ColliderComponent> _colliderComponentMapper = null!;

    /// <summary>
    /// Initializes a new instance of the player control system.
    /// </summary>
    /// <param name="properties">The properties of the level.</param>
    public PlayerControlSystem(LevelProperties properties) : base(Aspect.All(typeof(PlayerComponent),
        typeof(VelocityComponent)))
    {
        _properties = properties;
    }

    /// <inheritdoc/>
    public override void Initialize(IComponentMapperService mapperService)
    {
        _playerComponentMapper = mapperService.GetMapper<PlayerComponent>();
        _velocityComponentMapper = mapperService.GetMapper<VelocityComponent>();
        _colliderComponentMapper = mapperService.GetMapper<ColliderComponent>();
    }

    /// <summary>
    /// Updates each player's target velocity from the keyboard and applies an
    /// upward jump impulse when the jump key is pressed and the player can jump.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
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
                var colliderComponent = _colliderComponentMapper.Get(entityId);
                var mtv = colliderComponent.Mtv;

                if (mtv.X is not 0)
                {
                    velocityComponent.Value.X = Math.Sign(mtv.X) * _properties.WalkSpeed;
                }

                velocityComponent.Value.Y = -_properties.JumpSpeed;

                if (playerComponent.IsJumping) playerComponent.CanJump = false;

                playerComponent.IsJumping = true;
            }
        }
    }
}