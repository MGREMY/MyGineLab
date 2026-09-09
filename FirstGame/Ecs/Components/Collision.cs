using FirstGame.Collisions;
using Microsoft.Xna.Framework;

namespace FirstGame.Ecs.Components;

public class Collision
{
    public readonly PlayerActor Actor;
    public Vector2 Correction;

    public Collision(PlayerActor actor)
    {
        Actor = actor;
    }
}