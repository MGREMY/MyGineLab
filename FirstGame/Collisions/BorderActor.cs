using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace FirstGame.Collisions;

public class BorderActor : ICollisionActor
{
    public int Id { get; }
    public CollisionShape2D Shape { get; }

    public BorderActor(int id, CollisionShape2D collisionShape)
    {
        Id = id;
        Shape = collisionShape;
    }
}