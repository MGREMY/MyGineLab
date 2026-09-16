using Core;
using MonoGame.Extended.Screens;
using Platformer.Screens;

namespace Platformer;

/// <summary>
/// The main Platformer game.
/// </summary>
public class Game1 : BaseGame
{
    /// <summary>
    /// Gets the screen manager that owns and transitions between game screens.
    /// </summary>
    public static ScreenManager ScreenManager { get; } = new();

    /// <summary>
    /// Initializes a new instance of the game and registers the <see cref="ScreenManager"/>
    /// as a game component.
    /// </summary>
    public Game1() : base("Platformer", new GameOption())
    {
        Components.Add(ScreenManager);
    }

    /// <summary>
    /// Initializes the game and immediately shows the gameplay screen.
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();

        ScreenManager.ShowScreen(new GameplayScreen(this));
    }
}