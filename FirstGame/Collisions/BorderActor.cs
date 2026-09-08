using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace FirstGame.Collisions;

public class BorderActor : ICollisionActor
{
    public int Id { get; }
    public CollisionShape2D Shape { get; }

    public BorderActor(int id, BoundingBox2D boundingBox)
    {
        Id = id;
        Shape = new CollisionShape2D(boundingBox);
    }
}