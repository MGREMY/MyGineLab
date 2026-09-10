using Microsoft.Xna.Framework;

namespace Platformer.Engine;

public class Constants
{
    public const string TilemapWallLayer = "Walls";
    public const string TilemapDataLayer = "MapData";

    public const string TilemapDataSpawnPoint = "SpawnPoint";
    public static Vector2 TilemapDataPlayerSize { get; } = new(16);
    public const float TilemapDataPlayerGravityScale = 1.0f;

    public const string WallLayer = "Walls";
    public const string PlayerLayer = "Players";

    public const float MovementSpeed = 1200f;
    public const float PlayerSpeed = 300f;
    public const float JumpSpeed = 200f;
    public const float MaxFallSpeed = 300f;
    public const float MaxWallSlideSpeed = 25f;
    public const float Gravity = 800f;
}