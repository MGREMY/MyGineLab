using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;

namespace Platformer.GameObjects.Levels;

public class GameLevel
{
    public Tilemap Tilemap { get; }
    private readonly LevelProperties _properties;
    private readonly World _ecs;
    private readonly CollisionWorld2D _collisions;
    private readonly OrthographicCamera _camera;
    private readonly TilemapSpriteBatchRenderer _renderer;

    public GameLevel(
        Tilemap tilemap,
        World ecs,
        CollisionWorld2D collisions,
        OrthographicCamera camera,
        TilemapSpriteBatchRenderer renderer)
    {
        Tilemap = tilemap;
        _properties = LevelProperties.ParseFromTilemap(Tilemap);

        _ecs = ecs;
        _collisions = collisions;
        _camera = camera;
        _renderer = renderer;
    }

    public void Update(GameTime gameTime)
    {
        _ecs.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _renderer.Draw(spriteBatch, _camera);
        _ecs.Draw(gameTime);
    }
}