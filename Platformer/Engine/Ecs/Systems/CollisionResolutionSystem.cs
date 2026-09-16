using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;
using Platformer.GameObjects.Levels;

namespace Platformer.Engine.Ecs.Systems;

/// <summary>
/// Resolves the collisions each collider detected on the last physics query:
/// it pushes the entity out of the walls and adjusts its velocity and player
/// state accordingly.
/// </summary>
public class CollisionResolutionSystem : EntityUpdateSystem
{
    private readonly EngineOptions _options;

    private ComponentMapper<ColliderComponent> _colliderComponentMapper = null!;
    private ComponentMapper<Transform2Component> _transformComponentMapper = null!;
    private ComponentMapper<VelocityComponent> _velocityComponentMapper = null!;
    private ComponentMapper<PlayerComponent> _playerComponentMapper = null!;

    /// <summary>
    /// Initializes a new instance of the collision resolution system.
    /// </summary>
    /// <param name="options">The properties of the level.</param>
    public CollisionResolutionSystem(EngineOptions options) : base(Aspect.All(typeof(ColliderComponent),
        typeof(Transform2Component)))
    {
        _options = options;
    }

    /// <inheritdoc/>
    public override void Initialize(IComponentMapperService mapperService)
    {
        _colliderComponentMapper = mapperService.GetMapper<ColliderComponent>();
        _transformComponentMapper = mapperService.GetMapper<Transform2Component>();
        _velocityComponentMapper = mapperService.GetMapper<VelocityComponent>();
        _playerComponentMapper = mapperService.GetMapper<PlayerComponent>();
    }

    /// <summary>
    /// Pushes each entity out of its collisions, then zeroes or clamps the
    /// velocity along each pushed axis and updates the player's on-ground,
    /// on-ceiling, on-wall and jump state.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var colliderComponent = _colliderComponentMapper.Get(entityId);
            var transformComponent = _transformComponentMapper.Get(entityId);
            var mtv = colliderComponent.Mtv;

            if (mtv == Vector2.Zero) continue;

            transformComponent.Position += mtv;

            if (!_velocityComponentMapper.TryGet(entityId, out var velocityComponent)) continue;

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
                else if (velocityComponent.Value.Y > _options.WallSlideSpeed)
                {
                    velocityComponent.Value.Y = _options.WallSlideSpeed;
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