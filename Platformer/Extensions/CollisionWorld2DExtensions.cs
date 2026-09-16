using MonoGame.Extended.Collisions;

namespace Platformer.Extensions;

public static class CollisionWorld2DExtensions
{
    extension(CollisionWorld2D collisions)
    {
        public void DisableSelfCollisions(string layerName)
        {
            collisions.DisableCollisionBetweenLayers(layerName, layerName);
        }

        public void EnableSelfCollisions(string layerName)
        {
            collisions.EnableCollisionBetweenLayers(layerName, layerName);
        }
    }
}