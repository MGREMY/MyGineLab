using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Core.Input;

public class MouseInfo
{
    public MouseState PreviousState { get; private set; }
    public MouseState CurrentState { get; private set; }

    public Point Position
    {
        get => CurrentState.Position;
        set => SetPosition(value.X, value.Y);
    }

    public int X
    {
        get => CurrentState.X;
        set => SetPosition(value, CurrentState.Y);
    }

    public int Y
    {
        get => CurrentState.Y;
        set => SetPosition(CurrentState.X, value);
    }

    public Point PositionDelta => CurrentState.Position - PreviousState.Position;
    public int XDelta => CurrentState.X - PreviousState.X;
    public int YDelta => CurrentState.Y - PreviousState.Y;
    public bool WasMoved => PositionDelta != Point.Zero;
    public int ScrollWheel => CurrentState.ScrollWheelValue;
    public int ScrollWheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

    public MouseInfo()
    {
        PreviousState = new MouseState();
        CurrentState = Mouse.GetState();
    }

    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    public bool IsButtonDown(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => CurrentState.LeftButton is ButtonState.Pressed,
            MouseButton.Right => CurrentState.RightButton is ButtonState.Pressed,
            MouseButton.Middle => CurrentState.MiddleButton is ButtonState.Pressed,
            MouseButton.XButton1 => CurrentState.XButton1 is ButtonState.Pressed,
            MouseButton.XButton2 => CurrentState.XButton2 is ButtonState.Pressed,
            _ => false,
        };
    }

    public bool IsButtonUp(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => CurrentState.LeftButton is ButtonState.Released,
            MouseButton.Right => CurrentState.RightButton is ButtonState.Released,
            MouseButton.Middle => CurrentState.MiddleButton is ButtonState.Released,
            MouseButton.XButton1 => CurrentState.XButton1 is ButtonState.Released,
            MouseButton.XButton2 => CurrentState.XButton2 is ButtonState.Released,
            _ => false,
        };
    }

    public bool WasButtonJustPressed(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => CurrentState.LeftButton is ButtonState.Pressed
                                || PreviousState.LeftButton is ButtonState.Released,
            MouseButton.Right => CurrentState.RightButton is ButtonState.Pressed
                                 || PreviousState.RightButton is ButtonState.Released,
            MouseButton.Middle => CurrentState.MiddleButton is ButtonState.Pressed
                                  || PreviousState.MiddleButton is ButtonState.Released,
            MouseButton.XButton1 => CurrentState.XButton1 is ButtonState.Pressed
                                    || PreviousState.XButton1 is ButtonState.Released,
            MouseButton.XButton2 => CurrentState.XButton2 is ButtonState.Pressed
                                    || PreviousState.XButton2 is ButtonState.Released,
            _ => false,
        };
    }

    public bool WasButtonJustReleased(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => CurrentState.LeftButton is ButtonState.Released
                                || PreviousState.LeftButton is ButtonState.Pressed,
            MouseButton.Right => CurrentState.RightButton is ButtonState.Released
                                 || PreviousState.RightButton is ButtonState.Pressed,
            MouseButton.Middle => CurrentState.MiddleButton is ButtonState.Released
                                  || PreviousState.MiddleButton is ButtonState.Pressed,
            MouseButton.XButton1 => CurrentState.XButton1 is ButtonState.Released
                                    || PreviousState.XButton1 is ButtonState.Pressed,
            MouseButton.XButton2 => CurrentState.XButton2 is ButtonState.Released
                                    || PreviousState.XButton2 is ButtonState.Pressed,
            _ => false,
        };
    }

    public void SetPosition(int x, int y)
    {
        Mouse.SetPosition(x, y);
        CurrentState = new MouseState(
            x: x,
            y: y,
            scrollWheel: CurrentState.ScrollWheelValue,
            leftButton: CurrentState.LeftButton,
            middleButton: CurrentState.MiddleButton,
            rightButton: CurrentState.RightButton,
            xButton1: CurrentState.XButton1,
            xButton2: CurrentState.XButton2);
    }
}