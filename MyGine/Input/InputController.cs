using Microsoft.Xna.Framework;

namespace MyGine.Input;

public sealed class InputController
{
    private const int MaxGamePadSize = 8;

    public KeyboardInfo Keyboard { get; } = new();
    public MouseInfo Mouse { get; } = new();
    public GamePadInfo[] GamePads { get; } = new GamePadInfo[MaxGamePadSize];


    public InputController()
    {
        for (var i = 0; i < MaxGamePadSize; i++)
        {
            GamePads[i] = new GamePadInfo((PlayerIndex)i);
        }
    }

    public void Update(GameTime gameTime)
    {
        Keyboard.Update();
        Mouse.Update();

        for (var i = 0; i < MaxGamePadSize; i++)
        {
            GamePads[i].Update(gameTime);
        }
    }
}