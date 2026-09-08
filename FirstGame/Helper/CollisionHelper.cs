using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace FirstGame.Helper;

public static class CollisionHelper
{
    public static CollisionShape2D CreateFromPositionAndSize(float x, float y, float width, float height)
    {
        var boundingBox = BoundingBox2D.CreateFromPositionAndSize(
            position: new Vector2(x, y),
            size: new Vector2(width, height));

        return new CollisionShape2D(boundingBox);
    }
}