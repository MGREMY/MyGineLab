using System;
using System.Collections.Generic;
using System.Linq;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace FirstGame.Ecs.Systems;

public class NewCollisionSystem(CollisionWorld2D collisionWorld)
    : EntityUpdateSystem(Aspect.All(typeof(Collision), typeof(Transform2), typeof(Sprite)))
{
    private ComponentMapper<Collision> _collisionMapper = null!;
    private ComponentMapper<Transform2> _transformMapper = null!;
    private ComponentMapper<Sprite> _spriteMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _collisionMapper = mapperService.GetMapper<Collision>();
        _transformMapper = mapperService.GetMapper<Transform2>();
        _spriteMapper = mapperService.GetMapper<Sprite>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var collision = _collisionMapper.Get(entityId);
            var transform = _transformMapper.Get(entityId);
            var sprite = _spriteMapper.Get(entityId);

            var boundingBox = BoundingBox2D.CreateFromPositionAndSize(transform.Position, sprite.Size.ToVector2());
            var collisionShape = new CollisionShape2D(boundingBox);

            collision.Correction = Vector2.Zero;

            foreach (var actor in collisionWorld.QueryCandidates(boundingBox, "Walls"))
            {
                if (!actor.Shape.TryGetCollision(collisionShape, out var result)) continue;

                if (result.MinimumTranslationVector == Vector2.Zero) continue;
                
                var initialCorrectionX = result.MinimumTranslationVector.X;
                var initialCorrectionY = result.MinimumTranslationVector.Y;

                if (actor.Shape.TryGetCollision(
                        new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(
                            new Vector2(transform.Position.X, transform.Position.Y - initialCorrectionY),
                            sprite.Size.ToVector2())), out var resultForX))
                {
                    if (Math.Pow(resultForX.MinimumTranslationVector.X, 2) > Math.Pow(collision.Correction.X, 2))
                        collision.Correction.X = resultForX.MinimumTranslationVector.X;
                }

                if (actor.Shape.TryGetCollision(
                        new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(
                            new Vector2(transform.Position.X - initialCorrectionX, transform.Position.Y),
                            sprite.Size.ToVector2())), out var resultForY))
                {
                    if (Math.Pow(resultForY.MinimumTranslationVector.Y, 2) > Math.Pow(collision.Correction.Y, 2))
                        collision.Correction.Y = resultForY.MinimumTranslationVector.Y;
                }
            }

            transform.Position -= collision.Correction;
        }
    }
}

public class CollisionSystem(CollisionWorld2D world)
    : EntityUpdateSystem(Aspect.All(typeof(Transform2), typeof(Sprite)))
{
    private ComponentMapper<Transform2> _transformMapper = null!;
    private ComponentMapper<Velocity> _velocityMapper = null!;
    private ComponentMapper<Sprite> _spriteMapper = null!;
    private ComponentMapper<Control> _controllableMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<Velocity>();
        _spriteMapper = mapperService.GetMapper<Sprite>();
        _controllableMapper = mapperService.GetMapper<Control>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var transform = _transformMapper.Get(entityId);
            var sprite = _spriteMapper.Get(entityId);

            // First we check if there are any collisions
            // If there are any, we update the position
            var correctionVector = Vector2.Zero;

            {
                var correctionY = 0.0f;
                var correctionX = 0.0f;

                var boundingBox = BoundingBox2D.CreateFromPositionAndSize(transform.Position, sprite.Size.ToVector2());
                var collisionActors = world.QueryCandidates(boundingBox).ToArray();

                correctionVector = GetCorrectionVector(collisionActors, new CollisionShape2D(boundingBox));

                boundingBox = BoundingBox2D.CreateFromPositionAndSize(
                    new Vector2(transform.Position.X - correctionVector.X, transform.Position.Y),
                    sprite.Size.ToVector2());
                collisionActors = world.QueryCandidates(boundingBox).ToArray();
                correctionY = GetCorrectionVector(collisionActors, new CollisionShape2D(boundingBox)).Y;

                boundingBox = BoundingBox2D.CreateFromPositionAndSize(
                    new Vector2(transform.Position.X, transform.Position.Y - correctionVector.Y),
                    sprite.Size.ToVector2());
                collisionActors = world.QueryCandidates(boundingBox).ToArray();
                correctionX = GetCorrectionVector(collisionActors, new CollisionShape2D(boundingBox)).X;

                correctionVector = new Vector2(correctionX, correctionY);
            }

            // If no vector, it means there is no collision
            if (correctionVector == Vector2.Zero) return;

            // Apply the correction and move out of collisions
            transform.Position -= correctionVector;

            // If the entity is controllable, we'll update the velocity and the controls if needed
            if (_controllableMapper.TryGet(entityId, out var controllable)
                && _velocityMapper.TryGet(entityId, out var velocity))
            {
                // If collision => CanJump = true / IsJumping = false
                if (correctionVector != Vector2.Zero)
                {
                    controllable.IsJumping = false;
                    controllable.CanJump = true;
                }

                // If some collision happen on Y axis, we reset the vertical velocity
                if (correctionVector.Y is not 0) velocity.Value.Y = 0;

                // If some collision happen on X axis
                if (correctionVector.X is not 0)
                {
                    // If velocity wants to go up, we reset it to 0
                    if (velocity.Value.Y < 0)
                    {
                        velocity.Value.Y = 0;
                    }

                    // If the user wants to go on the wall, we reduce falling velocity Y
                    // And cancel horizontal velocity
                    // Simulate the player holding on a wall and sliding from it
                    if ((velocity.Target.X > 0 && correctionVector.X > 0)
                        || velocity.Target.X < 0 && correctionVector.X < 0)
                    {
                        velocity.Value.X = 0;
                        velocity.Value.Y *= 0.75f;
                    }

                    // If the user is jumping and wants to go up
                    // And if we have a collision on X axis
                    // We first need to move to the opposite side of the collision
                    if (correctionVector.X is not 0 && velocity.Target.Y < 0)
                    {
                        velocity.Value.X = -velocity.Target.X;
                    }
                }
            }
        }
    }

    private Vector2 GetCorrectionVector(IEnumerable<ICollisionActor> actors, CollisionShape2D collisionShape)
    {
        var correctionVector = Vector2.Zero;

        foreach (var actorCollisionShape in actors.Select(x => x.Shape))
        {
            if (!actorCollisionShape.TryGetCollision(collisionShape, out var result))
                continue;

            if (Math.Pow(result.MinimumTranslationVector.X, 2) > Math.Pow(correctionVector.X, 2))
                correctionVector.X = result.MinimumTranslationVector.X;
            if (Math.Pow(result.MinimumTranslationVector.Y, 2) > Math.Pow(correctionVector.Y, 2))
                correctionVector.Y = result.MinimumTranslationVector.Y;
        }

        return correctionVector;
    }
}