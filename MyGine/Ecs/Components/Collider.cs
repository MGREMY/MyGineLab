using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;

namespace MyGine.Ecs.Components;

public enum CollisionResponse
{
    None = 0,
    PushOut = 1,
    PushOutAndStop = 2,
}

public sealed class CollisionInfo2D
{
    public required CollisionResult2D[] Results { get; set; }
    public Entity? Other { get; set; }
}

public sealed class Collider2D : ICollisionActor
{
    public int Id { get; set; }
    public required CollisionShape2D Shape { get; set; }
    public Entity? Entity { get; set; }
    public string Layer { get; set; } = "World";
    public HashSet<string> Queries { get; } = [];
    public HashSet<CollisionEvent2D> PreviousCollisions { get; } = [];
    public HashSet<CollisionEvent2D> Collisions { get; } = [];
    public CollisionResponse CollisionResponse { get; set; } = CollisionResponse.PushOutAndStop;

    public event Action<CollisionInfo2D>? CollisionEnter;
    public event Action<CollisionInfo2D>? CollisionStay;
    public event Action<CollisionInfo2D>? CollisionExit;

    public Vector2 Mtv => Collisions.Aggregate(
        Vector2.Zero,
        (v, ce) => v + ce.Result.MinimumTranslationVector);

    public void RaiseCollisionEnter(CollisionInfo2D info) => CollisionEnter?.Invoke(info);
    public void RaiseCollisionStay(CollisionInfo2D info) => CollisionStay?.Invoke(info);
    public void RaiseCollisionExit(CollisionInfo2D info) => CollisionExit?.Invoke(info);

    public void UpdatePosition(Vector2 position) =>
        Shape = new CollisionShape2D(Shape.BoundingBox.Translate(position - Shape.BoundingBox.Min));
}