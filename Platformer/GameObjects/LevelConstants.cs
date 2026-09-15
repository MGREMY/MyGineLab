namespace Platformer.GameObjects;

public class LevelConstants
{
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