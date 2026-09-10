using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

public class CollisionResolutionSystem : EntityUpdateSystem
{
    private readonly LevelProperties _properties;

    private ComponentMapper<ColliderComponent> _colliderComponentMapper = null!;
    private ComponentMapper<Transform2Component> _transformComponentMapper = null!;
    private ComponentMapper<VelocityComponent> _velocityComponentMapper = null!;
    private ComponentMapper<PlayerComponent> _playerComponentMapper = null!;

    public CollisionResolutionSystem(LevelProperties properties) : base(Aspect.All(typeof(ColliderComponent),
        typeof(Transform2Component)))
    {
        _properties = properties;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _colliderComponentMapper = mapperService.GetMapper<ColliderComponent>();
        _transformComponentMapper = mapperService.GetMapper<Transform2Component>();
        _velocityComponentMapper = mapperService.GetMapper<VelocityComponent>();
        _playerComponentMapper = mapperService.GetMapper<PlayerComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var colliderComponent = _colliderComponentMapper.Get(entityId);
            var transformComponent = _transformComponentMapper.Get(entityId);

            var mtv = Vector2.Zero;

            foreach (var collisionEvent in colliderComponent.LastFrameCollisions)
            {
                if (Math.Abs(collisionEvent.Result.MinimumTranslationVector.X) > Math.Abs(mtv.X))
                    mtv.X = collisionEvent.Result.MinimumTranslationVector.X;
                if (Math.Abs(collisionEvent.Result.MinimumTranslationVector.Y) > Math.Abs(mtv.Y))
                    mtv.Y = collisionEvent.Result.MinimumTranslationVector.Y;
            }

            if (mtv != Vector2.Zero)
            {
                transformComponent.Position += mtv;

                if (_velocityComponentMapper.TryGet(entityId, out var velocityComponent))
                {
                    var playerComponentExists =
                        _playerComponentMapper.TryGet(entityId, out PlayerComponent? playerComponent);

                    // on ground
                    if (mtv.Y < 0)
                    {
                        if (playerComponentExists)
                        {
                            playerComponent.OnGround = true;
                            playerComponent.CanJump = true;
                            playerComponent.IsJumping = false;
                        }

                        velocityComponent.Value.Y = 0;
                        velocityComponent.Target.Y = 0;
                    }
                    else if (playerComponentExists)
                    {
                        playerComponent.OnGround = false;
                    }

                    // on ceiling
                    if (mtv.Y > 0)
                    {
                        if (playerComponentExists) playerComponent.OnCeiling = true;

                        velocityComponent.Value.Y = 0;
                        velocityComponent.Target.Y = 0;
                    }
                    else if (playerComponentExists)
                    {
                        playerComponent.OnCeiling = false;
                    }

                    // hit a wall
                    if (mtv.X is not 0.0f)
                    {
                        if (playerComponentExists)
                        {
                            playerComponent.OnWall = true;
                            playerComponent.CanJump = true;
                            playerComponent.IsJumping = false;
                        }

                        // if the velocity is going upwards
                        if (velocityComponent.Value.Y < 0)
                        {
                            velocityComponent.Value.Y = 0;
                            velocityComponent.Target.Y = 0;
                        }
                        // if the velocity is going down
                        else if (velocityComponent.Value.Y > _properties.WallSlideSpeed)
                        {
                            velocityComponent.Value.Y = _properties.WallSlideSpeed;
                        }

                        velocityComponent.Value.X = 0;
                        velocityComponent.Target.X = 0;
                    }
                    else if (playerComponentExists)
                    {
                        playerComponent.OnWall = false;
                    }
                }
            }
        }
    }
}