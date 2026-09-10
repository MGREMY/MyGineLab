using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

public class RenderSystem : EntityDrawSystem
{
    private readonly SpriteBatch _spriteBatch;
    private readonly OrthographicCamera _camera;

    private ComponentMapper<Transform2Component> _transformComponentMapper = null!;
    private ComponentMapper<Sprite> _spriteComponentMapper = null!;

    public RenderSystem(SpriteBatch spriteBatch, OrthographicCamera camera) : base(
        Aspect.All(typeof(Transform2Component), typeof(Sprite)))
    {
        _spriteBatch = spriteBatch;
        _camera = camera;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformComponentMapper = mapperService.GetMapper<Transform2Component>();
        _spriteComponentMapper = mapperService.GetMapper<Sprite>();
    }

    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

        foreach (var entityId in ActiveEntities)
        {
            var transformComponent = _transformComponentMapper.Get(entityId);
            var spriteComponent = _spriteComponentMapper.Get(entityId);

            if (_camera.Contains(transformComponent.Position) is not ContainmentType.Disjoint)
            {
                _spriteBatch.Draw(spriteComponent, transformComponent.Position);
            }
        }

        _spriteBatch.End();
    }
}