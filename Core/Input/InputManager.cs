using Microsoft.Xna.Framework;

namespace Core.Input;

public sealed class InputManager
{
    public static InputManager Instance
    {
        get
        {
            field ??= new InputManager();
            return field;
        }
    }

    private const int MaxGamePadSize = 8;

    public KeyboardInfo Keyboard { get; }
    public MouseInfo Mouse { get; }
    public GamePadInfo[] GamePads { get; }


    private InputManager()
    {
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();
        GamePads = new GamePadInfo[MaxGamePadSize];
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