using System.Linq;
using Core.Input;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace FirstGame.Ecs.Systems;

public class MovementSystem() : EntityUpdateSystem(Aspect.All(typeof(Transform2), typeof(Velocity)))
{
    private const float DecelerationFactor = 2.5f;

    private ComponentMapper<Transform2> _transformMapper = null!;
    private ComponentMapper<Velocity> _velocityMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<Velocity>();
    }

    public override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entityId in ActiveEntities)
        {
            var transform = _transformMapper.Get(entityId);
            var velocity = _velocityMapper.Get(entityId);

            transform.Position += velocity.Value * deltaTime;

            if (InputManager.Instance.Keyboard.CurrentState.GetPressedKeys()
                .Any(x => x is Keys.Z or Keys.Q or Keys.S or Keys.D)) continue;

            velocity.Value -= velocity.Value * DecelerationFactor * deltaTime;

            if (velocity.Value.LengthSquared() <= 5f) velocity.Value = Vector2.Zero;
        }
    }
}