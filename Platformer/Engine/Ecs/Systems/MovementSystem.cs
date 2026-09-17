using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;
using Platformer.GameObjects.Levels;

namespace Platformer.Engine.Ecs.Systems;

/// <summary>
/// Ramps each entity's horizontal velocity toward its target at a fixed
/// acceleration.
/// </summary>
public class MovementSystem : EntityUpdateSystem
{
    private readonly EngineOptions _options;

    private ComponentMapper<VelocityComponent> _velocityComponentMapper = null!;

    /// <summary>
    /// Initializes a new instance of the movement system.
    /// </summary>
    /// <param name="options">The properties of the level.</param>
    public MovementSystem(EngineOptions options) : base(Aspect.All(typeof(VelocityComponent)))
    {
        _options = options;
    }

    /// <inheritdoc/>
    public override void Initialize(IComponentMapperService mapperService)
    {
        _velocityComponentMapper = mapperService.GetMapper<VelocityComponent>();
    }

    /// <summary>
    /// Moves each entity's horizontal velocity toward its target by the level's
    /// movement acceleration, and snags near-zero velocities to zero.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entityId in ActiveEntities)
        {
            var velocityComponent = _velocityComponentMapper.Get(entityId);

            velocityComponent.Value.X = MoveTowards(
                current: velocityComponent.Value.X,
                target: velocityComponent.Target.X,
                amount: _options.MoveAcceleration * deltaTime);

            if (Math.Abs(velocityComponent.Value.X) <= 1f)
                velocityComponent.Value.X = 0.0f;
        }
    }

    private float MoveTowards(float current, float target, float amount)
    {
        if (Math.Abs(target - current) <= amount) return target;

        return current + Math.Sign(target - current) * amount;
    }
}