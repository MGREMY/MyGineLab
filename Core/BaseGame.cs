using System;
using Core.Audio;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core;

public class BaseGame : Game
{
    private static BaseGame Instance { get; set; } = null!;

    public GameOption GameOption { get; private set; }
    public GraphicsDeviceManager Graphics { get; }
    public SpriteBatch SpriteBatch { get; private set; } = null!;

    protected BaseGame(
        string title,
        GameOption option)
    {
        if (Instance is not null)
        {
            throw new InvalidOperationException("Only a single Base instance can be created");
        }

        Instance = this;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        // Set options and trigger once a OnChanged event to set the relevant settings
        GameOption = option;
        GameOption.OnOptionChanged += OnOptionChanged;
        GameOption.GraphicOption.OnGraphicOptionChanged += OnGraphicOptionChanged;
        OnOptionChanged(null, option);
        OnGraphicOptionChanged(null, option.GraphicOption);

        Window.Title = title;
    }

    protected override void Initialize()
    {
        base.Initialize();

        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (GameOption.ExitOnEscape && InputManager.Instance.Keyboard.IsKeyDown(Keys.Escape)) Exit();

        InputManager.Instance.Update(gameTime);
        AudioController.Instance.Update();
    }

    private void OnOptionChanged(object? sender, GameOption e)
    {
        GameOption = e;

        //IsFixedTimeStep = true;
        //TargetElapsedTime = TimeSpan.FromSeconds((float)1 / e.TargetTps);
        IsMouseVisible = e.IsMouseVisible;
    }

    private void OnGraphicOptionChanged(object? sender, GraphicOption e)
    {
        GameOption.GraphicOption = e;

        Graphics.SynchronizeWithVerticalRetrace = true;
        Graphics.PreferredBackBufferWidth = e.Width;
        Graphics.PreferredBackBufferHeight = e.Height;
        Graphics.IsFullScreen = e.IsFullScreen;

        Graphics.ApplyChanges();
    }
}