namespace Platformer.Engine.Ecs.Components;

/// <summary>
/// Holds the player's gameplay state, such as which surfaces it touches and
/// whether it is allowed to jump.
/// </summary>
public class PlayerComponent
{
    /// <summary>
    /// Gets or sets a value indicating whether the player is standing on a
    /// floor.
    /// </summary>
    public bool OnGround = false;

    /// <summary>
    /// Gets or sets a value indicating whether the player is touching a
    /// ceiling.
    /// </summary>
    public bool OnCeiling = false;

    /// <summary>
    /// Gets or sets a value indicating whether the player is touching a wall.
    /// </summary>
    public bool OnWall = false;

    /// <summary>
    /// Gets or sets a value indicating whether the player is allowed to jump.
    /// </summary>
    public bool CanJump = true;

    /// <summary>
    /// Gets or sets a value indicating whether the player is currently jumping.
    /// </summary>
    public bool IsJumping = false;
}