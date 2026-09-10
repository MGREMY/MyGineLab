using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Platformer.Engine.Ecs.Components;

public class ColliderComponent : ICollisionActor
{
    public int Id { get; }
    public CollisionShape2D Shape { get; private set; }

    public string LayerName;
    public CollisionEvent2D[] LastFrameCollisions = [];

    public ColliderComponent(int id, CollisionShape2D shape) : this(id, shape, string.Empty)
    {
        Id = id;
        Shape = shape;
    }

    public ColliderComponent(int id, CollisionShape2D shape, string layerName)
    {
        Id = id;
        Shape = shape;
        LayerName = layerName;
    }

    public void UpdatePosition(Vector2 position)
    {
        var currentPosition = Shape.BoundingBox.Min;
        var newPosition = Shape.BoundingBox.Translate(position - currentPosition);

        Shape = new CollisionShape2D(newPosition);
    }
}