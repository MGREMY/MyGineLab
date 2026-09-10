using System;
using System.Linq;
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

        var groupedNames = StringHelper.GroupByCommonPrefix('~', [.. atlas.Select(x => x.Name)]);

        foreach (var (key, regionNames) in groupedNames)
        {
            spriteSheet.DefineAnimation(key, builder =>
            {
                builder.IsLooping(true);

                foreach (var regionName in regionNames)
                {
                    builder.AddFrame(regionName, animationSpeed);
                }
            });
        }

        return new AnimatedSprite(spriteSheet, groupedNames.First().Key);
    }
}