using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MyGine.Screens;

namespace MyGine.MyGame.Screens;

public class GameScreen : SimulationScreen
{
    protected TilemapSpriteBatchRenderer Renderer { get; set; } = null!;
    protected Tilemap Map { get; set; } = null!;

    public GameScreen(BaseGame game, AppContext appContext) : base(game, appContext)
    {
    }

    public override void LoadContent()
    {
        Map = ContentManager.Load<TilemapWorld>(Path.Combine("map", "world")).Levels[0];
        Renderer = new TilemapSpriteBatchRenderer
        {
            BlendState = BlendState.AlphaBlend,
        };
        Renderer.LoadTilemap(Map);
        
        base.LoadContent();
    }

    protected override CollisionWorld2D BuildCollisionWorld()
    {
        var box = new SpatialHash(Map.TileToWorldPosition(Map.Width, Map.Height));
        var layer = new Layer(box) { IsDynamic = false };

        return new CollisionWorld2D(layer);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        Renderer.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        GraphicsDevice.Clear(Map.BackgroundColor ?? Color.Black);
        Renderer.Draw(SpriteBatch, SceneServices.Camera);
    }
}