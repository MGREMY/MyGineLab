using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;

namespace Platformer.Engine.Ecs.Components;

/// <summary>
/// Gives an entity a sprite to draw, either static or animated.
/// </summary>
public class SpriteComponent
{
    /// <summary>
    /// Gets or sets the sprite of the entity; an <see cref="AnimatedSprite"/> is
    /// advanced by the update methods.
    /// </summary>
    public Sprite Sprite;

    /// <summary>
    /// Initializes a new instance of a sprite component.
    /// </summary>
    /// <param name="sprite">The sprite of the entity.</param>
    public SpriteComponent(Sprite sprite)
    {
        Sprite = sprite;
    }

    /// <summary>
    /// Advances an animated sprite by the given amount of time, in seconds.
    /// </summary>
    /// <param name="amount">The elapsed time, in seconds.</param>
    public void Update(float amount)
    {
        if (Sprite is AnimatedSprite animatedSprite) animatedSprite.Update(TimeSpan.FromSeconds(amount));
    }

    /// <summary>
    /// Advances an animated sprite by the given elapsed time.
    /// </summary>
    /// <param name="elapsedTime">The elapsed time.</param>
    public void Update(TimeSpan elapsedTime)
    {
        if (Sprite is AnimatedSprite animatedSprite) animatedSprite.Update(elapsedTime);
    }

    /// <summary>
    /// Advances an animated sprite by the elapsed time of the given game time.
    /// </summary>
    /// <param name="gameTime">The game time elapsed since the last update.</param>
    public void Update(GameTime gameTime)
    {
        if (Sprite is AnimatedSprite animatedSprite) animatedSprite.Update(gameTime);
    }

    public void ChangeAnimation(string name)
    {
        if (Sprite is AnimatedSprite animatedSprite && animatedSprite.CurrentAnimation != name)
        {
            animatedSprite.SetAnimation(name);
        }
    }
}