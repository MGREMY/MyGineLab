using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace FirstGame.Collisions;

public class TileActor : ICollisionActor
{
    public int Id { get; }
    public CollisionShape2D Shape { get; }

    public TileActor(int id, int x, int y, int width, int height)
    {
        Id = id;
        Shape = new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(new Vector2(x, y), new Vector2(width, height)));
    }
}