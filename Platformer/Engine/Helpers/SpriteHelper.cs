using System;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Graphics;

namespace Platformer.Engine.Helpers;

public static class SpriteHelper
{
    public static AnimatedSprite CreateAnimatedSprite(ContentManager content, string name)
    {
        var atlas = content.Load<Texture2DAtlas>(name);
        var spriteSheet = new SpriteSheet(name, atlas);

        var animationSpeed = TimeSpan.FromSeconds(0.3);

        spriteSheet.DefineAnimation("idle", builder =>
        {
            builder.IsLooping(true)
                .AddFrame("adventurer-idle-00", animationSpeed)
                .AddFrame("adventurer-idle-01", animationSpeed)
                .AddFrame("adventurer-idle-02", animationSpeed);
        });

        spriteSheet.DefineAnimation("run", builder =>
        {
            builder.IsLooping(true)
                .AddFrame("adventurer-run-00", animationSpeed)
                .AddFrame("adventurer-run-01", animationSpeed)
                .AddFrame("adventurer-run-02", animationSpeed)
                .AddFrame("adventurer-run-03", animationSpeed)
                .AddFrame("adventurer-run-04", animationSpeed)
                .AddFrame("adventurer-run-05", animationSpeed);
        });

        return new AnimatedSprite(spriteSheet, "idle");
    }
}