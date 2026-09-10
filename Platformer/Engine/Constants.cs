using Microsoft.Xna.Framework;

namespace Platformer.Engine;

/// <summary>
/// Defines the default constants used by the engine and the fallback values for
/// the per-level properties.
/// </summary>
public class Constants
{
    // Engine dependencies
    /// <summary>
    /// Gets the name of the tilemap data layer.
    /// </summary>
    public const string TilemapDataLayer = "MapData";

    /// <summary>
    /// Gets the name of the tilemap layer describing the walls of the level.
    /// </summary>
    public const string TilemapWallLayer = "Walls";

    /// <summary>
    /// Gets the name of the data-layer object describing the spawn point.
    /// </summary>
    public const string TilemapDataDefinitionSpawnPoint = "SpawnPoint";

    /// <summary>
    /// Gets the size, in pixels, of the player used by the tilemap data.
    /// </summary>
    public static Vector2 TilemapDataPlayerSize { get; } = new(16);

    /// <summary>
    /// Gets the default gravity scale applied to the player.
    /// </summary>
    public const float TilemapDataPlayerGravityScale = 1.0f;

    /// <summary>
    /// Gets the default name of the collision layer used for walls.
    /// </summary>
    public const string WallLayer = "Walls";

    /// <summary>
    /// Gets the default name of the collision layer used for players.
    /// </summary>
    public const string PlayerLayer = "Players";

    // Level properties default values
    /// <summary>
    /// Gets the default width of the virtual camera viewport in pixels.
    /// </summary>
    public const int CameraWidth = 640;

    /// <summary>
    /// Gets the default height of the virtual camera viewport in pixels.
    /// </summary>
    public const int CameraHeight = 320;

    /// <summary>
    /// Gets the default width of the player's hitbox in pixels.
    /// </summary>
    public const float PlayerWidth = 16f;

    /// <summary>
    /// Gets the default height of the player's hitbox in pixels.
    /// </summary>
    public const float PlayerHeight = 16f;

    /// <summary>
    /// Gets the default gravity acceleration in pixels per second squared.
    /// </summary>
    public const float Gravity = 800f;

    /// <summary>
    /// Gets the default gravity scale multiplier.
    /// </summary>
    public const float GravityScale = 1f;

    /// <summary>
    /// Gets the default movement acceleration in pixels per second squared.
    /// </summary>
    public const float MoveAcceleration = 1200f;

    /// <summary>
    /// Gets the default maximum horizontal walking speed in pixels per second.
    /// </summary>
    public const float WalkSpeed = 300f;

    /// <summary>
    /// Gets the default jump speed in pixels per second.
    /// </summary>
    public const float JumpSpeed = 200f;

    /// <summary>
    /// Gets the default maximum falling speed in pixels per second.
    /// </summary>
    public const float MaxFallSpeed = 300f;

    /// <summary>
    /// Gets the default maximum downward sliding speed on a wall in pixels per
    /// second.
    /// </summary>
    public const float WallSlideSpeed = 25f;
}