using System;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace FirstGame.Ecs.Systems;

public class CameraSystem(Camera<Vector2> camera)
    : EntityUpdateSystem(Aspect.All(typeof(CameraTarget), typeof(Transform2)))
{
    private ComponentMapper<CameraTarget> _cameraTargetMapper = null!;
    private ComponentMapper<Transform2> _transformMapper = null!;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _cameraTargetMapper = mapperService.GetMapper<CameraTarget>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        if (ActiveEntities.Count is not 1)
            throw new InvalidOperationException("Cannot have more or less than one camera target");

        var cameraTarget = _cameraTargetMapper.Get(ActiveEntities[0]);
        var transform = _transformMapper.Get(ActiveEntities[0]);

        camera.Zoom = cameraTarget.Zoom;
        // Camera dead zone
        {
            var target = camera.Center;

            var dx = transform.Position.X - target.X;
            var dy = transform.Position.Y - target.Y;

            if (Math.Abs(dx) > cameraTarget.DeadZone)
                target.X = transform.Position.X - Math.Sign(dx) * cameraTarget.DeadZone;
            if (Math.Abs(dy) > cameraTarget.DeadZone)
                target.Y = transform.Position.Y - Math.Sign(dy) * cameraTarget.DeadZone;

            camera.LookAt(target);
        }
    }
}