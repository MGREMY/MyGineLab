using Core.Input;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace FirstGame.Ecs.Systems;

public class InputSystem() : EntityUpdateSystem(Aspect.All(typeof(Velocity)))
{
    private const float Acceleration = 300f;

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

            if (InputManager.Instance.Keyboard.IsKeyDown(Keys.Z)) velocity.Value.Y -= Acceleration * deltaTime;
            if (InputManager.Instance.Keyboard.IsKeyDown(Keys.S)) velocity.Value.Y += Acceleration * deltaTime;
            if (InputManager.Instance.Keyboard.IsKeyDown(Keys.Q)) velocity.Value.X -= Acceleration * deltaTime;
            if (InputManager.Instance.Keyboard.IsKeyDown(Keys.D)) velocity.Value.X += Acceleration * deltaTime;
        }
    }
}