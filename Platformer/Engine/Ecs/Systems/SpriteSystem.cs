using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

/// <summary>
/// Advances every entity's animated sprite over time.
/// </summary>
public class SpriteSystem : EntityUpdateSystem
{
    private ComponentMapper<SpriteComponent> _spriteComponentMapper = null!;

    /// <summary>
    /// Initializes a new instance of the sprite system.
    /// </summary>
    public SpriteSystem() : base(Aspect.All(typeof(SpriteComponent)))
    {
    }

    /// <inheritdoc/>
    public override void Initialize(IComponentMapperService mapperService)
    {
        _spriteComponentMapper = mapperService.GetMapper<SpriteComponent>();
    }

    /// <summary>
    /// Advances every entity's animated sprite by the elapsed game time.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var spriteComponent = _spriteComponentMapper.Get(entityId);

            spriteComponent.Update(gameTime);
        }
    }
}