using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame.Helper;

public static class TextureHelper
{
    public static Texture2D CreateFromSize(GraphicsDevice graphicsDevice, Point size, Color color)
    {
        var texture = new Texture2D(graphicsDevice, size.X, size.Y);
        var data = new Color[size.X * size.Y];

        for (var i = 0; i < data.Length; i++)
            data[i] = color;

        texture.SetData(data);

        return texture;
    }
}