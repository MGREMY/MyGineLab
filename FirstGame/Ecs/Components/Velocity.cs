using Microsoft.Xna.Framework;

namespace FirstGame.Ecs.Components;

public record Velocity
{
    public Vector2 Value = Vector2.Zero;
    public Vector2 Target = Vector2.Zero;
    public float GravitySpeed = 100f;
}