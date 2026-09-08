using System;
using System.Linq;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;

namespace FirstGame.Ecs.Systems;

public class CollisionSystem(CollisionWorld2D world)
    : EntityUpdateSystem(Aspect.All(typeof(Transform2), typeof(Velocity), typeof(Sprite)))
{
    private ComponentMapper<Transform2> _transformMapper = null!;
    private ComponentMapper<Velocity> _velocityMapper = null!;
    private ComponentMapper<Sprite> _spriteMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<Velocity>();
        _spriteMapper = mapperService.GetMapper<Sprite>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var transform = _transformMapper.Get(entityId);
            var sprite = _spriteMapper.Get(entityId);

            var boundingBox = BoundingBox2D.CreateFromPositionAndSize(transform.Position, sprite.Size.ToVector2());
            var collisionActors = world.QueryCandidates(boundingBox, "Walls").ToArray();

            if (collisionActors.Length is 0) return;

            var velocity = _velocityMapper.Get(entityId);
            var collisionShape = new CollisionShape2D(boundingBox);
            var correctionX = 0.0f;
            var correctionY = 0.0f;

            foreach (var actor in collisionActors)
            {
                if (!actor.Shape.TryGetCollision(collisionShape, out var result))
                    continue;

                if (Math.Abs(result.MinimumTranslationVector.X) > Math.Abs(correctionX))
                    correctionX = result.MinimumTranslationVector.X;
                if (Math.Abs(result.MinimumTranslationVector.Y) > Math.Abs(correctionY))
                    correctionY = result.MinimumTranslationVector.Y;
            }

            var translationVector = new Vector2(correctionX, correctionY);

            if (translationVector == Vector2.Zero) return;

            if (Math.Sign(velocity.Target.X) is not 0 && Math.Sign(velocity.Target.X) != Math.Sign(correctionX)) velocity.CanGoUp = true;

            velocity.Value.Y = 0;

            if (Vector2.Normalize(translationVector).X - Vector2.UnitX.X is 0) velocity.Value.X = 0;

            transform.Position -= translationVector;
        }
    }
}