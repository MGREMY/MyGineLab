using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;

namespace Platformer.Engine.Ecs.Components;

public class SpriteComponent
{
    public Sprite Sprite;

    public SpriteComponent(Sprite sprite)
    {
        Sprite = sprite;
    }

    public void Update(float amount)
    {
        if (Sprite is AnimatedSprite animatedSprite) animatedSprite.Update(TimeSpan.FromSeconds(amount));
    }

    public void Update(TimeSpan elapsedTime)
    {
        if (Sprite is AnimatedSprite animatedSprite) animatedSprite.Update(elapsedTime);
    }

    public void Update(GameTime gameTime)
    {
        if (Sprite is AnimatedSprite animatedSprite) animatedSprite.Update(gameTime);
    }
}