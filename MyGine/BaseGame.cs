using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MyGine.Audio;
using MyGine.Input;

namespace MyGine;

public abstract class BaseGame : Game
{
    protected ScreenManager ScreenManager { get; private set; } = new();
    protected GraphicsDeviceManager Graphics { get; private set; }
    protected SpriteBatch SpriteBatch { get; private set; } = null!;
    protected AppContext AppContext { get; private set; }

    public BaseGame()
    {
        Graphics = new GraphicsDeviceManager(this);

        AppContext = new AppContext
        {
            InputController = new InputController(),
            AudioController = new AudioController(),
        };

        Components.Add(ScreenManager);
    }

    protected override void Initialize()
    {
        base.Initialize();

        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        AppContext.InputController.Update(gameTime);
        AppContext.AudioController.Update();
    }
}