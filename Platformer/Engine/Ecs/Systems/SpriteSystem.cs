using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

public class SpriteSystem : EntityUpdateSystem
{
    private ComponentMapper<SpriteComponent> _spriteComponentMapper = null!;

    public SpriteSystem() : base(Aspect.All(typeof(SpriteComponent)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _spriteComponentMapper = mapperService.GetMapper<SpriteComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var spriteComponent = _spriteComponentMapper.Get(entityId);

            spriteComponent.Update(gameTime);
        }
    }
}