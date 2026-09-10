using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using Platformer.Engine.Collisions;

namespace Platformer.Engine;

public class LevelData
{
    public required Tilemap Tilemap { get; init; }
    public required TilemapSpriteBatchRenderer Renderer { get; init; }
    public required WallActor[] Walls { get; init; }
    public required int[] EntityIds { get; init; }
    public required Vector2 SpawnPoint { get; init; }
    public required Size SizePx { get; init; }
}