using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace FirstGame.Collisions;

public class TileActor : ICollisionActor
{
    public int Id { get; }
    public CollisionShape2D Shape { get; }

    public TileActor(int id, CollisionShape2D collisionShape)
    {
        Id = id;
        Shape = collisionShape;
    }
}