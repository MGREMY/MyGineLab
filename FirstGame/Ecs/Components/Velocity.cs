using Microsoft.Xna.Framework;

namespace FirstGame.Ecs.Components;

public class Velocity()
{
    public bool CanGoUp = true;
    
    public Vector2 Value = Vector2.Zero;
    public Vector2 Target = Vector2.Zero;
}