using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyGine.Input;

public class GamePadInfo
{
    private TimeSpan _remainingVibrationTime = TimeSpan.Zero;

    public PlayerIndex PlayerIndex { get; }
    public GamePadState PreviousState { get; private set; }
    public GamePadState CurrentState { get; private set; }

    public bool IsConnected => CurrentState.IsConnected;
    public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;
    public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;
    public float LeftTrigger => CurrentState.Triggers.Left;
    public float RightTrigger => CurrentState.Triggers.Right;

    public GamePadInfo(PlayerIndex playerIndex)
    {
        PlayerIndex = playerIndex;
        PreviousState = new GamePadState();
        CurrentState = GamePad.GetState(PlayerIndex);
    }

    public void Update(GameTime gameTime)
    {
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(PlayerIndex);

        if (_remainingVibrationTime <= TimeSpan.Zero) return;

        _remainingVibrationTime -= gameTime.ElapsedGameTime;
        if (_remainingVibrationTime <= TimeSpan.Zero) StopVibration();
    }

    public bool IsButtonDown(Buttons button)
    {
        return CurrentState.IsButtonDown(button);
    }

    public bool IsButtonUp(Buttons button)
    {
        return CurrentState.IsButtonUp(button);
    }

    public bool WasButtonPressed(Buttons button)
    {
        return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
    }

    public bool WasJustReleased(Buttons button)
    {
        return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
    }

    public void SetVibration(float strength, TimeSpan duration)
    {
        _remainingVibrationTime = duration;
        GamePad.SetVibration(PlayerIndex, strength, strength);
    }

    public void StopVibration()
    {
        GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
    }
}