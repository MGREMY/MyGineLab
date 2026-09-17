using System;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Graphics;
using Platformer.Engine.Helpers;

namespace Platformer.Helpers;

/// <summary>
/// Provides helpers used to build sprites out of texture atlas assets.
/// </summary>
public static class SpriteHelper
{
    /// <summary>
    /// Creates an <see cref="AnimatedSprite"/> from a texture atlas following
    /// the engine's frame-naming convention.
    /// </summary>
    /// <remarks>
    /// The atlas regions must be named <c>&lt;animationKey&gt;~NN</c> (for
    /// example <c>adventurer-idle~00</c>, <c>adventurer-run~00…05</c>). One
    /// looping animation is created per unique <c>&lt;animationKey&gt;</c>, whose
    /// frames play in counter order. The current animation is the first key in
    /// sort order (for the player sheet that is <c>adventurer-idle</c>); a
    /// way to choose the default animation through a parameter is planned once
    /// the engine is ready. All frames share a single duration.
    /// </remarks>
    /// <param name="content">The content manager used to load the atlas.</param>
    /// <param name="name">The name of the texture atlas asset.</param>
    /// <returns>The created animated sprite.</returns>
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