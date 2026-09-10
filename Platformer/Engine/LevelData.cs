using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using Platformer.Engine.Collisions;

namespace Platformer.Engine;

/// <summary>
/// Describes the contents of a built level: its tilemap, renderer, static
/// collision walls, entity ids, spawn point and size.
/// </summary>
public class LevelData
{
    /// <summary>
    /// Gets the tilemap describing the level geometry.
    /// </summary>
    public required Tilemap Tilemap { get; init; }

    /// <summary>
    /// Gets the renderer used to draw the level's tilemap.
    /// </summary>
    public required TilemapSpriteBatchRenderer Renderer { get; init; }

    /// <summary>
    /// Gets the static collision walls of the level.
    /// </summary>
    public required WallActor[] Walls { get; init; }

    /// <summary>
    /// Gets the ids of the entities created for the level.
    /// </summary>
    public required int[] EntityIds { get; init; }

    /// <summary>
    /// Gets the player spawn point in pixels.
    /// </summary>
    public required Vector2 SpawnPoint { get; init; }

    /// <summary>
    /// Gets the size of the level in pixels.
    /// </summary>
    public required Size SizePx { get; init; }
}