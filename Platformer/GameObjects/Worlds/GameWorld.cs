using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tilemaps;
using Platformer.GameObjects.Levels;

namespace Platformer.GameObjects.Worlds;

public class GameWorld
{
    private readonly TilemapWorld _tilemapWorld;
    private readonly Sprite _playerSprite;
    private readonly GameWindow _window;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    public GameLevel? ActiveLevel { get; private set; }

    public EventHandler? OnLoading { get; set; }
    public EventHandler? OnLoaded { get; set; }

    public GameWorld(
        TilemapWorld tilemapWorld,
        Sprite playerSprite,
        GameWindow window,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch)
    {
        _tilemapWorld = tilemapWorld;
        _playerSprite = playerSprite;
        _window = window;
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
    }

    public void LoadLevel(int index)
    {
        LoadLevel(_tilemapWorld.Levels[index]);
    }

    public void LoadLevel(Tilemap tilemap)
    {
        OnLoading?.Invoke(this, EventArgs.Empty);

        ActiveLevel = new GameLevelBuilder(
                tilemap,
                LevelProperties.ParseFromTilemap(tilemap),
                _playerSprite,
                _window,
                _graphicsDevice,
                _spriteBatch)
            .WithWalls()
            .WithEntities()
            .WithPlayer()
            .Build();

        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void Update(GameTime gameTime)
    {
        ActiveLevel?.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        ActiveLevel?.Draw(gameTime, _spriteBatch);
    }
}