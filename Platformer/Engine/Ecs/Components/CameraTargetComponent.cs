namespace Platformer.Engine.Ecs.Components;

/// <summary>
/// Marks an entity as a following target for the camera and holds the camera
/// tuning associated with it.
/// </summary>
public class CameraTargetComponent
{
    /// <summary>
    /// Gets or sets the zoom level the camera should use when following the entity.
    /// </summary>
    public float Zoom = 1.0f;

    /// <summary>
    /// Gets or sets the dead zone radius, in pixels, inside which the camera does
    /// not move.
    /// </summary>
    public float DeadZone = 120f;
}