using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Core;

public abstract class BaseScreen(BaseGame game) : GameScreen(game)
{
    protected GameOption GameOption => game.GameOption;
    protected SpriteBatch SpriteBatch => game.SpriteBatch;
}