using Microsoft.Xna.Framework;

namespace Platformer.Engine;

public class LevelProperties
{
    // Engine/Map properties, immutable
    public string CollisionWallLayer => Constants.WallLayer;
    public string CollisionPlayerLayer => Constants.PlayerLayer;

    public string TilemapWallLayer => Constants.TilemapWallLayer;
    public string TilemapDataLayer => Constants.TilemapDataLayer;
    public string TilemapDataDefinitionSpawnPoint => Constants.TilemapDataDefinitionSpawnPoint;

    // Level properties, mutable
    public required Point VirtualCameraSize { get; set; }
    public required float PlayerWidth { get; set; }
    public required float PlayerHeight { get; set; }
    public required float Gravity { get; set; }
    public required float GravityScale { get; set; }
    public required float MoveAcceleration { get; set; }
    public required float WalkSpeed { get; set; }
    public required float JumpSpeed { get; set; }
    public required float WallSlideSpeed { get; set; }
    public required float MaxFallSpeed { get; set; }
}