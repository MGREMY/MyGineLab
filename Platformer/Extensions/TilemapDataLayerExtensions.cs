using MonoGame.Extended.Tilemaps;

namespace Platformer.Extensions;

public static class TilemapDataLayerExtensions
{
    extension(TilemapObjectLayer objectLayer)
    {
        public TilemapObject? GetObjectByName(string name)
        {
            foreach (var o in objectLayer.Objects)
            {
                if (o.Name == name) return o;
            }

            return null;
        }
    }
}