using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MyGine.Ecs.Components;

namespace MyGine.Ecs.Systems;

public sealed class RendererSystem2D : EntityDrawSystem
{
    private readonly SceneServices _services;

    private ComponentMapper<Drawable2D> _drawableMapper = null!;
    private ComponentMapper<Transform2> _transformMapper = null!;

    public RendererSystem2D(SceneServices services)
        : base(Aspect.All(typeof(Drawable2D), typeof(Transform2)))
    {
        _services = services;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _drawableMapper = mapperService.GetMapper<Drawable2D>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Draw(GameTime gameTime)
    {
        if (_services.Camera is not null)
        {
            var transformMatrix = _services.Camera.GetViewMatrix()
                                  * Matrix.CreateScale(xScale: 1, yScale: -1, zScale: 1)
                                  * Matrix.CreateTranslation(
                                      xPosition: 0,
                                      yPosition: _services.Camera.Origin.Y * 2,
                                      zPosition: 0);

            _services.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);   
        }
        else
        {
            _services.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        }

        foreach (var entityId in ActiveEntities)
        {
            var transform = _transformMapper.Get(entityId);
            var drawable = _drawableMapper.Get(entityId);

            if (_services.Camera?.BoundingRectangle.Contains(transform.Position) is false) continue;
            if (!drawable.Sprite.IsVisible) continue;

            // If we have an effect FlipVertically, we need to draw without it
            // If we don't have it, we need to draw with it
            // All because of the transformMatrix done to convert the engine Y-UP coordinate system
            // to Y-down coordinate system of the sprite batch
            _services.SpriteBatch.Draw(
                textureRegion: drawable.Sprite.TextureRegion,
                position: transform.Position,
                color: drawable.Sprite.Color * drawable.Sprite.Alpha,
                rotation: transform.Rotation,
                origin: drawable.Sprite.Origin,
                scale: transform.Scale * _services.App.Settings.MeterPerPixelFactor,
                effects: drawable.Sprite.Effect ^ SpriteEffects.FlipVertically,
                layerDepth: drawable.Sprite.Depth);
        }

        _services.SpriteBatch.End();
    }
}