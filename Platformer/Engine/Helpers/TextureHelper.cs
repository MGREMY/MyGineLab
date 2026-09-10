using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer.Engine.Helpers;

/// <summary>
/// Provides helpers used to create procedural textures.
/// </summary>
public static class TextureHelper
{
    /// <summary>
    /// Creates a solid-color texture of the given size.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device used to create the texture.</param>
    /// <param name="size">The size of the texture in pixels.</param>
    /// <param name="color">The color of every pixel of the texture.</param>
    /// <returns>The created texture.</returns>
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