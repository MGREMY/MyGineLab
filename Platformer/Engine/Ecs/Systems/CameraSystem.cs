using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Engine.Ecs.Components;

namespace Platformer.Engine.Ecs.Systems;

/// <summary>
/// Moves the camera toward its target and applies its zoom. The camera follows
/// the target only once it leaves the target's dead zone.
/// </summary>
/// <remarks>
/// Exactly one entity may carry a <see cref="CameraTargetComponent"/>; the
/// system throws otherwise.
/// </remarks>
public class CameraSystem : EntityUpdateSystem
{
    private readonly OrthographicCamera _camera;

    private ComponentMapper<CameraTargetComponent> _cameraTargetComponentMapper = null!;
    private ComponentMapper<Transform2Component> _transformComponentMapper = null!;

    /// <summary>
    /// Initializes a new instance of the camera system.
    /// </summary>
    /// <param name="camera">The camera to move.</param>
    public CameraSystem(OrthographicCamera camera) : base(Aspect.All(typeof(CameraTargetComponent),
        typeof(Transform2Component)))
    {
        _camera = camera;
    }

    /// <inheritdoc/>
    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformComponentMapper = mapperService.GetMapper<Transform2Component>();
        _cameraTargetComponentMapper = mapperService.GetMapper<CameraTargetComponent>();
    }

    /// <summary>
    /// Moves the camera toward its target inside the target's dead zone and
    /// applies its zoom.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public override void Update(GameTime gameTime)
    {
        if (ActiveEntities.Count is not 1)
            throw new InvalidOperationException("Cannot have more or less than one camera target");

        var cameraTargetComponent = _cameraTargetComponentMapper.Get(ActiveEntities[0]);
        var transformComponent = _transformComponentMapper.Get(ActiveEntities[0]);

        _camera.Zoom = cameraTargetComponent.Zoom;
        // Camera dead zone
        {
            var target = _camera.Center;

            var dx = transformComponent.Position.X - target.X;
            var dy = transformComponent.Position.Y - target.Y;

            if (Math.Abs(dx) > cameraTargetComponent.DeadZone)
                target.X = transformComponent.Position.X - Math.Sign(dx) * cameraTargetComponent.DeadZone;
            if (Math.Abs(dy) > cameraTargetComponent.DeadZone)
                target.Y = transformComponent.Position.Y - Math.Sign(dy) * cameraTargetComponent.DeadZone;

            _camera.LookAt(target);
        }
    }
}