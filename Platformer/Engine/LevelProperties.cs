using Microsoft.Xna.Framework;

namespace Platformer.Engine;

/// <summary>
/// Holds the tunable properties of a level, loaded from the level's map
/// properties with <see cref="Constants"/> values as fallbacks.
/// </summary>
public class LevelProperties
{
    // Engine/Map properties, immutable

    /// <summary>
    /// Gets the name of the collision layer used for walls.
    /// </summary>
    public string CollisionWallLayer => Constants.WallLayer;

    /// <summary>
    /// Gets the name of the collision layer used for players.
    /// </summary>
    public string CollisionPlayerLayer => Constants.PlayerLayer;

    /// <summary>
    /// Gets the name of the tilemap layer describing the walls of the level.
    /// </summary>
    public string TilemapWallLayer => Constants.TilemapWallLayer;

    /// <summary>
    /// Gets the name of the tilemap data layer.
    /// </summary>
    public string TilemapDataLayer => Constants.TilemapDataLayer;

    /// <summary>
    /// Gets the name of the data-layer object describing the spawn point.
    /// </summary>
    public string TilemapDataDefinitionSpawnPoint => Constants.TilemapDataDefinitionSpawnPoint;

    // Level properties, mutable

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
}