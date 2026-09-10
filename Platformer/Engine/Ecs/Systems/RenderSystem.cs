using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;
using Platformer.Engine.Extensions;

namespace Platformer.Engine.Ecs.Systems;

public class RenderSystem : EntityDrawSystem
{
    private LevelProperties _properties;
    private readonly SpriteBatch _spriteBatch;
    private readonly OrthographicCamera _camera;

    private ComponentMapper<Transform2Component> _transformComponentMapper = null!;
    private ComponentMapper<SpriteComponent> _spriteComponentMapper = null!;

    public RenderSystem(LevelProperties properties, SpriteBatch spriteBatch, OrthographicCamera camera) : base(
        Aspect.All(typeof(Transform2Component), typeof(SpriteComponent)))
    {
        _properties = properties;
        _spriteBatch = spriteBatch;
        _camera = camera;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformComponentMapper = mapperService.GetMapper<Transform2Component>();
        _spriteComponentMapper = mapperService.GetMapper<SpriteComponent>();
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
                var bounds = new RectangleF(
                    position: transformComponent.Position,
                    size: new SizeF(_properties.PlayerWidth, _properties.PlayerHeight));

                _spriteBatch.DrawInsideStretch(spriteComponent.Sprite, bounds);
            }
        }

        _spriteBatch.End();
    }
}