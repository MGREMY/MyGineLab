using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace FirstGame.Ecs.Systems;

public class DebugRenderSystem(SpriteBatch spriteBatch, Camera<Vector2> camera, SpriteFont font)
    : EntityDrawSystem(Aspect.All(typeof(Transform2), typeof(Velocity)))
{
    private ComponentMapper<Transform2> _transformMapper = null!;
    private ComponentMapper<Velocity> _velocityMapper = null!;
    private ComponentMapper<Control> _controllableMapper = null!;
    private ComponentMapper<Collision> _collisionMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<Velocity>();
        _controllableMapper = mapperService.GetMapper<Control>();
        _collisionMapper = mapperService.GetMapper<Collision>();
    }

    public override void Draw(GameTime gameTime)
    {
        spriteBatch.Begin();

        var lastDrawVector = Vector2.Zero;

        foreach (var entityId in ActiveEntities)
        {
            var transform = _transformMapper.Get(entityId);
            var velocity = _velocityMapper.Get(entityId);

            var positionText = $"Position: {{ X: {transform.Position.X:F4}; Y: {transform.Position.Y:F4} }}";
            var velocityValueText = $"Velocity => Value: {{ X: {velocity.Value.X:F4}; Y: {velocity.Value.Y:F4} }}";
            var velocityTargetText = $"Velocity => Target: {{ X: {velocity.Target.X:F4}; Y: {velocity.Target.Y:F4} }}";

            spriteBatch.DrawString(
                spriteFont: font,
                text: positionText,
                position: lastDrawVector,
                color: Color.White);
            lastDrawVector.Y += font.MeasureString(positionText).Y;
            spriteBatch.DrawString(
                spriteFont: font,
                text: velocityValueText,
                position: lastDrawVector,
                color: Color.Blue);
            lastDrawVector.Y += font.MeasureString(velocityValueText).Y;
            spriteBatch.DrawString(
                spriteFont: font,
                text: velocityTargetText,
                position: lastDrawVector,
                color: Color.White);
            lastDrawVector.Y += font.MeasureString(velocityTargetText).Y;

            if (_controllableMapper.TryGet(entityId, out var controllable))
            {
                var txt =
                    $"Controllable: {{ CanJump: {controllable.CanJump}; IsJumping: {controllable.IsJumping}; CanCrouch: {controllable.CanCrouch}; IsCrouched: {controllable.IsCrouched} }}";
                spriteBatch.DrawString(
                    spriteFont: font,
                    text: txt,
                    position: lastDrawVector,
                    color: Color.White);
                lastDrawVector.Y += font.MeasureString(txt).Y;
            }

            if (_collisionMapper.TryGet(entityId, out var collision))
            {
                var txt =
                    $"Collision: {{ X: {collision.Correction.X:F4}; Y: {collision.Correction.Y:F4} }}";
                spriteBatch.DrawString(
                    spriteFont: font,
                    text: txt,
                    position: lastDrawVector,
                    color: Color.White);
                lastDrawVector.Y += font.MeasureString(txt).Y;
            }
        }

        spriteBatch.End();

        spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

        foreach (var entityId in ActiveEntities)
        {
            var transform = _transformMapper.Get(entityId);
            var velocity = _velocityMapper.Get(entityId);

            spriteBatch.DrawLine(transform.Position, transform.Position + velocity.Value, Color.Blue);
        }

        spriteBatch.End();
    }
}