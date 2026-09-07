using Core;
using FirstGame.Screens;
using MonoGame.Extended.Screens;

namespace FirstGame;

public class Game1 : BaseGame
{
    public ScreenManager ScreenManager { get; } = new();

    public Game1() : base("FirstGame", new GameOption())
    {
        Components.Add(ScreenManager);
    }

    protected override void Initialize()
    {
        base.Initialize();
        ScreenManager.ShowScreen(new GameplayScreen(this));
    }
}