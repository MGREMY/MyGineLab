using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;

namespace Platformer.Engine.Extensions;

public static class SpriteBatchExtensions
{
    extension(SpriteBatch spriteBatch)
    {
        /// <summary>
        /// Draws a sprite inside a designated rectangle, scaling the texture inside.
        /// The sprite is horizontally centered and bottom anchored
        /// </summary>
        /// <param name="sprite">The sprite to draw.</param>
        /// <param name="bounds">The bounds where the sprite must be drawn.</param>
        public void DrawInside(Sprite sprite, RectangleF bounds)
        {
            var region = sprite.TextureRegion;

            // For rotated texture, width and height are swapped
            var frameWidth = region.Width;
            var frameHeight = region.Height;
            if (region.IsRotated) (frameWidth, frameHeight) = (frameHeight, frameWidth);

            var scale = Math.Min(bounds.Width / frameWidth, bounds.Height / frameHeight);

            var drawWidth = frameWidth * scale;
            var drawHeight = frameHeight * scale;

            // bottom anchors and horizontal center
            var x = bounds.X + (bounds.Width - drawWidth) * 0.5f;
            var y = bounds.Y + (bounds.Height - drawHeight);

            var color = sprite.Color * sprite.Alpha;

            if (region.IsRotated)
            {
                // region.Bounds is the pre-rotated storage; rotate -90° back upright.
                // Stored (region.Width - 1, 0) is the authored frame's top-left corner.
                spriteBatch.Draw(
                    texture: region.Texture,
                    position: new Vector2(x, y),
                    sourceRectangle: region.Bounds,
                    color: color,
                    rotation: -MathHelper.PiOver2,
                    origin: new Vector2(region.Width - 1, 0),
                    scale: new Vector2(scale),
                    effects: SpriteEffects.None,
                    layerDepth: 1.0f);
            }
            else
            {
                spriteBatch.Draw(
                    texture: region.Texture,
                    position: new Vector2(x, y),
                    sourceRectangle: region.Bounds,
                    color: color,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: new Vector2(scale),
                    effects: SpriteEffects.None,
                    layerDepth: 0f);
            }
        }

        public void DrawInsideStretch(Sprite sprite, RectangleF bounds)
        {
            var region = sprite.TextureRegion;
            var color = sprite.Color * sprite.Alpha;

            if (region.IsRotated)
            {
                var frameWidth = region.Width;
                var frameHeight = region.Height;

                // authored frame dims are stored swapped; stretch back to the box
                spriteBatch.Draw(
                    texture: region.Texture,
                    position: new Vector2(bounds.X, bounds.Y),
                    sourceRectangle: region.Bounds,
                    color: color,
                    rotation: -MathHelper.PiOver2,
                    origin: new Vector2(region.Width - 1, 0),
                    scale: new Vector2(bounds.Height / frameHeight, bounds.Width / frameWidth),
                    effects: SpriteEffects.None,
                    layerDepth: 1.0f);
            }
            else
            {
                spriteBatch.Draw(
                    texture: region.Texture,
                    position: new Vector2(bounds.X, bounds.Y),
                    sourceRectangle: region.Bounds,
                    color: color,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: new Vector2(bounds.Width / region.Width, bounds.Height / region.Height),
                    effects: SpriteEffects.None,
                    layerDepth: 0f);
            }
        }
    }
}