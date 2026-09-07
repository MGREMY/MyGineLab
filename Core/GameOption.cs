using System;

namespace Core;

public record GraphicOption
{
    public EventHandler<GraphicOption>? OnGraphicOptionChanged;
    public bool HasChanged { get; private set; }

    public int Width
    {
        get;
        set
        {
            HasChanged = true;
            field = value;
        }
    } = 1280;

    public int Height
    {
        get;
        set
        {
            HasChanged = true;
            field = value;
        }
    } = 720;

    public bool IsFullScreen
    {
        get;
        set
        {
            HasChanged = true;
            field = value;
        }
    } = false;

    public float MaxFps
    {
        get;
        set
        {
            HasChanged = true;
            field = value;
        }
    } = 60;

    public void ApplyChanges(object? sender)
    {
        if (HasChanged) OnGraphicOptionChanged?.Invoke(sender, this);
    }
}

public record GameOption
{
    public EventHandler<GameOption>? OnOptionChanged;

    public bool HasChanged
    {
        get => field && GraphicOption.HasChanged;
        private set;
    }

    public bool IsMouseVisible
    {
        get;
        set
        {
            HasChanged = true;
            field = value;
        }
    } = true;

    public bool ExitOnEscape
    {
        get;
        set
        {
            HasChanged = true;
            field = value;
        }
    } = true;

    public GraphicOption GraphicOption
    {
        get;
        set
        {
            HasChanged = true;
            field = value;
        }
    } = new();

    public void ApplyChanges(object sender)
    {
        if (HasChanged) OnOptionChanged?.Invoke(sender, this);
    }
}