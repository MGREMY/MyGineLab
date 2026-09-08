using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;

namespace FirstGame.Ecs.Systems;

public class RenderSystem(SpriteBatch spriteBatch, Camera<Vector2> camera)
    : EntityDrawSystem(Aspect.All(typeof(Transform2), typeof(Sprite)))
{
    private ComponentMapper<Transform2> _transformMapper = null!;
    private ComponentMapper<Sprite> _spriteMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _spriteMapper = mapperService.GetMapper<Sprite>();
    }

    public override void Draw(GameTime gameTime)
    {
        spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

        foreach (var entityId in ActiveEntities)
        {
            var transform = _transformMapper.Get(entityId);
            var sprite = _spriteMapper.Get(entityId);

            if (camera.Contains(transform.Position) is not ContainmentType.Disjoint)
            {
                spriteBatch.Draw(sprite, transform);
            }
        }

        spriteBatch.End();
    }
}