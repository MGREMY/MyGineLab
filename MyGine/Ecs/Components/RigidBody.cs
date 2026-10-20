using Microsoft.Xna.Framework;

namespace MyGine.Ecs.Components;

public sealed class RigidBody2D
{
    public Vector2 Velocity { get; set; } = Vector2.Zero;
    public float Mass { get; set; } = 1f;
    public float GravityScale { get; set; } = 1f;
}

public sealed class RigidBody3D
{
    public Vector3 Velocity { get; set; } = Vector3.Zero;
    public float Mass { get; set; } = 1f;
    public float GravityScale { get; set; } = 1f;
}