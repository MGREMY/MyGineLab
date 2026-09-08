using System.IO;
using Core;
using FirstGame.GameObjects;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps;

namespace FirstGame.Screens;

public class GameplayScreen : BaseScreen
{
    private GameWorld _gameWorld = null!;

    public GameplayScreen(BaseGame game) : base(game)
    {
    }

    public override void LoadContent()
    {
        base.LoadContent();

        var tilemapWorld = Content.Load<TilemapWorld>(Path.Combine("map", "world"));

        _gameWorld = new GameWorld(tilemapWorld);
        _gameWorld.LoadLevel(0, Window, GraphicsDevice, SpriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        _gameWorld.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_gameWorld.ActiveLevel?.Tilemap.BackgroundColor ?? Color.Black);

        _gameWorld.Draw(gameTime, SpriteBatch);
    }
}