using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MyGine.Scripting;

namespace MyGine.Ecs.Systems;

public sealed class ScriptSystem : EntityUpdateSystem
{
    private readonly SceneServices _services;

    private ComponentMapper<Scripts> _scriptMapper = null!;

    public ScriptSystem(SceneServices services)
        : base(Aspect.All(typeof(Scripts)))
    {
        _services = services;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _scriptMapper = mapperService.GetMapper<Scripts>();
    }

    protected override void OnEntityAdded(int entityId) => AttachAllIfNeeded(entityId);

    protected override void OnEntityChanged(int entityId) => AttachAllIfNeeded(entityId);

    protected override void OnEntityRemoved(int entityId)
    {
        if (!_scriptMapper.TryGet(entityId, out var scripts)) return;

        foreach (var script in scripts)
        {
            script.OnDetached();
            script.Entity = null!;
            script.Services = null!;
        }
    }

    private void AttachAllIfNeeded(int entityId)
    {
        if (!_scriptMapper.TryGet(entityId, out var scripts)) return;

        var entity = GetEntity(entityId);

        foreach (var script in scripts)
        {
            script.Entity = entity;
            script.Services = _services;
            script.OnAttached();
        }
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var scripts = _scriptMapper.Get(entityId);

            foreach (var script in scripts)
            {
                script.Update(gameTime);
            }
        }
    }
}