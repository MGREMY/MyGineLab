using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Platformer.Engine.Ecs.Components;

/// <summary>
/// Gives an entity a collider in the collision world and stores the collisions
/// it detected on the last frame.
/// </summary>
public class ColliderComponent : ICollisionActor
{
    /// <summary>
    /// Gets the stable identity of this collider.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the collision shape of this collider.
    /// </summary>
    public CollisionShape2D Shape { get; private set; }

    /// <summary>
    /// Gets or sets the name of the collision layer this collider belongs to.
    /// </summary>
    public string LayerName;

    /// <summary>
    /// Gets or sets the collisions this collider detected on the last frame.
    /// </summary>
    public CollisionEvent2D[] LastFrameCollisions;

    public Vector2 Mtv => LastFrameCollisions.Aggregate(
        Vector2.Zero,
        (v, ce) => v + ce.Result.MinimumTranslationVector);

    /// <summary>
    /// Initializes a new instance of a collider with no layer.
    /// </summary>
    /// <param name="id">The stable identity of the collider.</param>
    /// <param name="shape">The collision shape of the collider.</param>
    public ColliderComponent(int id, CollisionShape2D shape) : this(id, shape, string.Empty)
    {
        Id = id;
        Shape = shape;
    }

    /// <summary>
    /// Initializes a new instance of a collider in the given layer.
    /// </summary>
    /// <param name="id">The stable identity of the collider.</param>
    /// <param name="shape">The collision shape of the collider.</param>
    /// <param name="layerName">The name of the collision layer.</param>
    public ColliderComponent(int id, CollisionShape2D shape, string layerName) : this(id, shape, layerName, [])
    {
    }

    /// <summary>
    /// Initializes a new instance of a collider in the given layer, with the
    /// collisions it detected on the previous frame.
    /// </summary>
    /// <param name="id">The stable identity of the collider.</param>
    /// <param name="shape">The collision shape of the collider.</param>
    /// <param name="layerName">The name of the collision layer.</param>
    /// <param name="lastFrameCollisions">The collisions detected on the previous frame.</param>
    public ColliderComponent(int id, CollisionShape2D shape, string layerName, CollisionEvent2D[] lastFrameCollisions)
    {
        Id = id;
        Shape = shape;
        LayerName = layerName;
        LastFrameCollisions = lastFrameCollisions;
    }

    /// <summary>
    /// Moves the collider's shape so its top-left corner matches the given
    /// position, preserving its size.
    /// </summary>
    /// <param name="position">The new top-left position of the collider, in pixels.</param>
    public void UpdatePosition(Vector2 position)
    {
        var currentPosition = Shape.BoundingBox.Min;
        var newPosition = Shape.BoundingBox.Translate(position - currentPosition);

        Shape = new CollisionShape2D(newPosition);
    }
}