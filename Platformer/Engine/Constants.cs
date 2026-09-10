using Microsoft.Xna.Framework;

namespace Platformer.Engine;

public class Constants
{
    // Engine dependencies
    public const string TilemapDataLayer = "MapData";
    public const string TilemapWallLayer = "Walls";
    public const string TilemapDataDefinitionSpawnPoint = "SpawnPoint";
    public static Vector2 TilemapDataPlayerSize { get; } = new(16);
    public const float TilemapDataPlayerGravityScale = 1.0f;

    public const string WallLayer = "Walls";
    public const string PlayerLayer = "Players";

    // Level properties default values
    public const int CameraWidth = 640;
    public const int CameraHeight = 320;
    public const float PlayerWidth = 16f;
    public const float PlayerHeight = 16f;
    public const float Gravity = 800f;
    public const float GravityScale = 1f;
    public const float MoveAcceleration = 1200f;
    public const float WalkSpeed = 300f;
    public const float JumpSpeed = 200f;
    public const float MaxFallSpeed = 300f;
    public const float WallSlideSpeed = 25f;
}