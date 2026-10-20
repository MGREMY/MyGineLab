using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using MyGine.Ecs.Systems;

namespace MyGine.Screens;

public abstract class SimulationScreen : Screen
{
    protected BaseGame Game { get; private set; }
    protected SceneServices SceneServices { get; set; } = null!;
    protected AppContext AppContext { get; set; }

    protected GameWindow Window => Game.Window;
    protected GameServiceContainer Services => Game.Services;
    protected ContentManager ContentManager => Game.Content;
    protected GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
    protected SpriteBatch SpriteBatch => new(GraphicsDevice);

    protected SimulationScreen(BaseGame game, AppContext appContext)
    {
        Game = game;
        AppContext = appContext;
    }

    public override void LoadContent()
    {
        base.LoadContent();
        
        SceneServices = new SceneServices
        {
            App = AppContext,
            ContentManager = ContentManager,
            GraphicsDevice = GraphicsDevice,
            SpriteBatch = SpriteBatch,
        };
        SceneServices.EcsWorld = BuildEcsWorld(SceneServices);
        SceneServices.CollisionWorld = BuildCollisionWorld();
        SceneServices.Camera = BuildCamera();
    }

    protected virtual World BuildEcsWorld(SceneServices sceneServices)
    {
        return new WorldBuilder()
            .AddSystem(new ScriptSystem(sceneServices))
            .AddSystem(new PhysicsSystem2D(sceneServices))
            .AddSystem(new CollisionSyncSystem2D())
            .AddSystem(new CollisionQuerySystem2D(sceneServices))
            .AddSystem(new CollisionResolutionSystem2D())
            .AddSystem(new CollisionEventSystem2D())
            .AddSystem(new RendererSystem2D(sceneServices))
            .Build();
    }

    protected abstract CollisionWorld2D BuildCollisionWorld();

    protected virtual OrthographicCamera BuildCamera()
    {
        var boxingViewportAdapter = new BoxingViewportAdapter(
            window: Window,
            graphicsDevice: GraphicsDevice,
            virtualWidth: 640,
            virtualHeight: 320);

        return new OrthographicCamera(boxingViewportAdapter)
        {
            Position = new Vector2(0, 0),
        };
    }

    public override void Update(GameTime gameTime)
    {
        SceneServices.EcsWorld.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        SceneServices.EcsWorld.Draw(gameTime);
    }
}