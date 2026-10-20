using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;

namespace MyGine.Scripting;

public abstract class Script
{
    public Entity Entity { get; set; } = null!;
    public SceneServices Services { get; set; } = null!;

    protected T? Get<T>() where T : class => Entity.Get<T>();
    protected bool Has<T>() where T : class => Entity.Has<T>();

    public virtual void OnAttached()
    {
    }

    public virtual void OnDetached()
    {
    }

    public virtual void Update(GameTime gameTime)
    {
    }
}