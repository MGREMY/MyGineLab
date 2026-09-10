using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Platformer.Engine.Collisions;

/// <summary>
/// A static collision actor representing a wall in the collision world.
/// </summary>
public class WallActor : ICollisionActor
{
    /// <summary>
    /// Gets the stable identity of this wall actor.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the collision shape of this wall.
    /// </summary>
    public CollisionShape2D Shape { get; }

    /// <summary>
    /// Initializes a new instance of a wall actor.
    /// </summary>
    /// <param name="id">The stable identity of the wall actor.</param>
    /// <param name="bounds">The collision shape of the wall.</param>
    public WallActor(int id, CollisionShape2D bounds)
    {
        Id = id;
        Shape = bounds;
    }
}