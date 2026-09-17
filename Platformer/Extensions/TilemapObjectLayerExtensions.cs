using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Tilemaps;

namespace Platformer.Extensions;

public static class TilemapObjectLayerExtensions
{
    extension(TilemapObjectLayer objectLayer)
    {
        public IEnumerable<T> GetObjects<T>(string name)
            where T : TilemapObject
        {
            return objectLayer.GetObjects<T>()
                .Where(x => x.Name == name);
        }
    }
}