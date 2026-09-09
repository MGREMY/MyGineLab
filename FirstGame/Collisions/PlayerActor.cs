using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace FirstGame.Collisions;

public class PlayerActor : ICollisionActor
{
    public int Id { get; }
    public CollisionShape2D Shape { get; private set; }

    public PlayerActor(int id, CollisionShape2D shape)
    {
        Id = id;
        Shape = shape;
    }

    public void UpdatePosition(Vector2 position)
    {
        Shape = new CollisionShape2D(Shape.BoundingBox.Translate(position - Shape.BoundingBox.Min));
    }
}