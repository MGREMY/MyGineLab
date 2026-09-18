using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;
using Platformer.Engine.Extensions;

namespace Platformer.Engine.Ecs.Systems;

/// <summary>
/// Draws every entity's sprite through the camera, stretched to fill its
/// hitbox exactly.
/// </summary>
public class RenderSystem : EntityDrawSystem
{
    private readonly SpriteBatch _spriteBatch;
    private readonly OrthographicCamera _camera;

    private ComponentMapper<Transform2Component> _transformComponentMapper = null!;
    private ComponentMapper<SpriteComponent> _spriteComponentMapper = null!;
    private ComponentMapper<VelocityComponent> _velocityComponentMapper = null!;

    /// <summary>
    /// Initializes a new instance of the render system.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used to render the entities.</param>
    /// <param name="camera">The camera used to transform the drawing.</param>
    public RenderSystem(SpriteBatch spriteBatch, OrthographicCamera camera) : base(
        Aspect.All(typeof(Transform2Component), typeof(SpriteComponent)))
    {
        _spriteBatch = spriteBatch;
        _camera = camera;
    }

    /// <inheritdoc/>
    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformComponentMapper = mapperService.GetMapper<Transform2Component>();
        _spriteComponentMapper = mapperService.GetMapper<SpriteComponent>();
        _velocityComponentMapper = mapperService.GetMapper<VelocityComponent>();
    }

    /// <summary>
    /// Draws each entity's sprite stretched inside its hitbox, skipping
    /// entities outside the camera's view.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

        foreach (var entityId in ActiveEntities)
        {
            var transformComponent = _transformComponentMapper.Get(entityId);
            var spriteComponent = _spriteComponentMapper.Get(entityId);

            if (_camera.Contains(transformComponent.Position) is not ContainmentType.Disjoint)
            {
                var bounds = new RectangleF(
                    position: transformComponent.Position,
                    size: transformComponent.Size);

                if (_velocityComponentMapper.TryGet(entityId, out var velocityComponent)
                    && velocityComponent.Value.X < 0)
                {
                    _spriteBatch.DrawInsideStretch(spriteComponent.Sprite, bounds, SpriteEffects.FlipHorizontally);
                }
                else
                {
                    _spriteBatch.DrawInsideStretch(spriteComponent.Sprite, bounds);
                }
            }
        }

        _spriteBatch.End();
    }
}