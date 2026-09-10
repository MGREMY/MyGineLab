namespace Platformer.Engine.Ecs.Components;

public class PlayerComponent
{
    public bool OnGround = false;
    public bool OnCeiling = false;
    public bool OnWall = false;
    public bool CanJump = true;
    public bool IsJumping = false;
}