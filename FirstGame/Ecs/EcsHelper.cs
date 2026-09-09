using FirstGame.Collisions;
using FirstGame.Ecs.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;

namespace FirstGame.Ecs;

public static class EcsHelper
{
    public static int CreatePlayer(World ecs, Vector2 position, Texture2D texture, PlayerActor actor)
    {
        var entity = ecs.CreateEntity();

        entity.Attach(new Collision(actor));
        entity.Attach(new Control(canJump: true, canCrouch: true));
        entity.Attach(new Transform2(position));
        entity.Attach(new Velocity());
        entity.Attach(new Sprite(texture));
        entity.Attach(new CameraTarget());

        return entity.Id;
    }
}