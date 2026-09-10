using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Platformer.Engine.Collisions;

public class WallActor : ICollisionActor
{
    public int Id { get; }
    public CollisionShape2D Shape { get; }

    public WallActor(int id, CollisionShape2D bounds)
    {
        Id = id;
        Shape = bounds;
    }
}