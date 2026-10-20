using System;
using MyGine.Audio;
using MyGine.Input;

namespace MyGine;

public sealed class Settings
{
    public int PixelPerMeter
    {
        get;
        init
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(PixelPerMeter), "Value cannot be less than or equal to 0");

            field = value;
            MeterPerPixelFactor = 1f / field;
        }
    } = 16;

    public float MeterPerPixelFactor { get; private set; } = 1f / 16f;

    public float GravityAcceleration { get; set; } = -9.81f;
    public float TerminalVelocity { get; set; } = -55f;
}

public sealed class AppContext
{
    public required InputController InputController { get; set; }
    public required AudioController AudioController { get; set; }

    public Settings Settings { get; set; } = new();
}