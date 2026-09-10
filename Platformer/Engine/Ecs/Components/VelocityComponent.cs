using Microsoft.Xna.Framework;

namespace Platformer.Engine.Ecs.Components;

/// <summary>
/// Holds an entity's current velocity and the velocity it is moving toward,
/// in pixels per second.
/// </summary>
public class VelocityComponent
{
    /// <summary>
    /// Gets or sets the current velocity of the entity.
    /// </summary>
    public Vector2 Value = Vector2.Zero;

    /// <summary>
    /// Gets or sets the target velocity the entity ramps toward.
    /// </summary>
    public Vector2 Target = Vector2.Zero;
}