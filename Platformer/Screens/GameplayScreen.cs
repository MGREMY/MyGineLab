using System.IO;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tilemaps;
using Platformer.Engine;

namespace Platformer.Screens;

public class GameplayScreen(BaseGame game) : BaseScreen(game)
{
    private GameWorld _gameWorld = null!;

    public override void LoadContent()
    {
        base.LoadContent();

        var font = Content.Load<SpriteFont>(Path.Combine("fonts", "roboto"));
        var tilemapWorld = Content.Load<TilemapWorld>(Path.Combine("map", "world"));

        _gameWorld = new GameWorld(tilemapWorld, font);
        _gameWorld.LoadLevel(0, Window, GraphicsDevice, Content, SpriteBatch);
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