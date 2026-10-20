using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MyGine.Ecs.Components;

namespace MyGine.Ecs.Systems;

public sealed class CollisionEventSystem2D : EntityUpdateSystem
{
    private ComponentMapper<Collider2D> _colliderMapper = null!;

    private HashSet<int> _previousIds = [];
    private HashSet<int> _currentIds = [];

    public CollisionEventSystem2D()
        : base(Aspect.All(typeof(Collider2D)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _colliderMapper = mapperService.GetMapper<Collider2D>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entityId in ActiveEntities)
        {
            var collider = _colliderMapper.Get(entityId);

            _previousIds.Clear();
            foreach (var c in collider.PreviousCollisions) _previousIds.Add(c.OtherId);

            _currentIds.Clear();
            foreach (var c in collider.Collisions) _currentIds.Add(c.OtherId);

            foreach (var group in collider.Collisions.GroupBy(x => x.Other))
            {
                var info = new CollisionInfo2D
                {
                    Results = [.. group.Select(x => x.Result)],
                    Other = (group.Key as Collider2D)?.Entity,
                };

                if (_previousIds.Contains(group.Key.Id)) collider.RaiseCollisionStay(info);
                else collider.RaiseCollisionEnter(info);
            }

            foreach (var group in collider.PreviousCollisions.GroupBy(x => x.Other))
            {
                if (_currentIds.Contains(group.Key.Id)) continue;

                collider.RaiseCollisionExit(new CollisionInfo2D
                {
                    Results = [.. group.Select(x => x.Result)],
                    Other = (group.Key as Collider2D)?.Entity,
                });
            }

            collider.PreviousCollisions.Clear();
            foreach (var collision in collider.Collisions) collider.PreviousCollisions.Add(collision);
        }
    }
}