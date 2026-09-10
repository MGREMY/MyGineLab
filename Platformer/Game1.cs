using Core;
using MonoGame.Extended.Screens;
using Platformer.Screens;

namespace Platformer;

public class Game1 : BaseGame
{
    public ScreenManager ScreenManager { get; } = new();

    public Game1() : base("Platformer", new GameOption())
    {
        Components.Add(ScreenManager);
    }

    protected override void Initialize()
    {
        base.Initialize();

        ScreenManager.ShowScreen(new GameplayScreen(this));
    }
}