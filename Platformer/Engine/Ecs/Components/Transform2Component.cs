using Microsoft.Xna.Framework;

namespace Platformer.Engine.Ecs.Components;

/// <summary>
/// Describes the position and size of an entity in world space, in pixels.
/// The position is the top-left corner of the entity's box.
/// </summary>
public class Transform2Component
{
    /// <summary>
    /// Gets or sets the position of the entity's top-left corner, in pixels.
    /// </summary>
    public Vector2 Position = Vector2.Zero;

    /// <summary>
    /// Gets or sets the size of the entity's box, in pixels.
    /// </summary>
    public Vector2 Size = Vector2.One;
}