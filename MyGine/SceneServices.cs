using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;

namespace MyGine;

public sealed class SceneServices
{
    public required AppContext App { get; set; }

    public required ContentManager ContentManager { get; set; }
    public required GraphicsDevice GraphicsDevice { get; set; }
    public required SpriteBatch SpriteBatch { get; set; }

    public World EcsWorld { get; set; } = null!;
    public CollisionWorld2D CollisionWorld { get; set; } = null!;
    public OrthographicCamera Camera { get; set; } = null!;
}