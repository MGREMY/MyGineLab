namespace FirstGame.Ecs.Components;

public record Control
{
    public bool CanJump;
    public bool IsJumping;
    public bool CanCrouch;
    public bool IsCrouched;

    public Control(
        bool canJump = false,
        bool isJumping = false,
        bool canCrouch = false,
        bool isCrouched = false)
    {
        CanJump = canJump;
        IsJumping = isJumping;
        CanCrouch = canCrouch;
        IsCrouched = isCrouched;
    }
}