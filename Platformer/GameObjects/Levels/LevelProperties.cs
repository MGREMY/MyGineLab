using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps;

namespace Platformer.GameObjects.Levels;

public class LevelProperties
{
    /// <summary>
    /// Gets or sets the size of the virtual camera viewport in pixels.
    /// </summary>
    public required Point VirtualCameraSize { get; set; }

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

    private LevelProperties()
    {
    }

    public static LevelProperties ParseFromTilemap(Tilemap map)
    {
        return new LevelProperties
        {
            VirtualCameraSize = new Point(
                map.Properties.GetInt("VirtualCameraWidth", LevelConstants.CameraWidth),
                map.Properties.GetInt("VirtualCameraHeight", LevelConstants.CameraHeight)),
            PlayerSize = new Vector2(
                map.Properties.GetFloat("PlayerWidth", LevelConstants.PlayerWidth),
                map.Properties.GetFloat("PlayerHeight", LevelConstants.PlayerHeight)),
            Gravity = map.Properties.GetFloat("Gravity", LevelConstants.Gravity),
            GravityScale = map.Properties.GetFloat("GravityScale", LevelConstants.GravityScale),
            MoveAcceleration = map.Properties.GetFloat("MoveAcceleration", LevelConstants.MoveAcceleration),
            WalkSpeed = map.Properties.GetFloat("WalkSpeed", LevelConstants.WalkSpeed),
            JumpSpeed = map.Properties.GetFloat("JumpSpeed", LevelConstants.JumpSpeed),
            WallSlideSpeed = map.Properties.GetFloat("WallSlideSpeed", LevelConstants.WallSlideSpeed),
            MaxFallSpeed = map.Properties.GetFloat("MaxFallSpeed", LevelConstants.MaxFallSpeed),
        };
    }
}