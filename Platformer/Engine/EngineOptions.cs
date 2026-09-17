using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps;

namespace Platformer.Engine;

public class EngineOptions
{
    /// <summary>
    /// Gets or sets the size of the player's hitbox in pixels.
    /// </summary>
    public required Vector2 PlayerSize { get; set; }

    /// <summary>
    /// Gets or sets the gravity acceleration applied to the player, in pixels per second squared.
    /// </summary>
    public required float Gravity { get; set; }

    /// <summary>
    /// Gets or sets the gravity scale multiplier applied to <see cref="Gravity"/>.
    /// </summary>
    public required float GravityScale { get; set; }

    /// <summary>
    /// Gets or sets the acceleration used to ramp the player's horizontal
    /// velocity toward its target, in pixels per second squared.
    /// </summary>
    public required float MoveAcceleration { get; set; }

    /// <summary>
    /// Gets or sets the maximum horizontal walking speed of the player, in
    /// pixels per second.
    /// </summary>
    public required float WalkSpeed { get; set; }

    /// <summary>
    /// Gets or sets the initial jump speed of the player, in pixels per second.
    /// </summary>
    public required float JumpSpeed { get; set; }

    /// <summary>
    /// Gets or sets the maximum downward sliding speed on a wall, in pixels per second.
    /// </summary>
    public required float WallSlideSpeed { get; set; }

    /// <summary>
    /// Gets or sets the maximum falling speed of the player, in pixels per second.
    /// </summary>
    public required float MaxFallSpeed { get; set; }

    private EngineOptions()
    {
    }

    public static readonly EngineOptions Default = new()
    {
        PlayerSize = new Vector2(16, 16),
        Gravity = 800f,
        GravityScale = 1f,
        MoveAcceleration = 1200f,
        WalkSpeed = 300f,
        JumpSpeed = 200f,
        MaxFallSpeed = 300f,
        WallSlideSpeed = 25f,
    };

    public static EngineOptions ParseFromTilemap(Tilemap map)
    {
        return new EngineOptions
        {
            PlayerSize = new Vector2(
                map.Properties.GetFloat("PlayerWidth", Default.PlayerSize.X),
                map.Properties.GetFloat("PlayerHeight", Default.PlayerSize.Y)),
            Gravity = map.Properties.GetFloat("Gravity", Default.Gravity),
            GravityScale = map.Properties.GetFloat("GravityScale", Default.GravityScale),
            MoveAcceleration = map.Properties.GetFloat("MoveAcceleration", Default.MoveAcceleration),
            WalkSpeed = map.Properties.GetFloat("WalkSpeed", Default.WalkSpeed),
            JumpSpeed = map.Properties.GetFloat("JumpSpeed", Default.JumpSpeed),
            WallSlideSpeed = map.Properties.GetFloat("WallSlideSpeed", Default.WallSlideSpeed),
            MaxFallSpeed = map.Properties.GetFloat("MaxFallSpeed", Default.MaxFallSpeed),
        };
    }
}