using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tilemaps;
using Platformer.Engine;

namespace Platformer.GameObjects.Levels;

public class LevelProperties
{
    public EngineOptions Options { get; }
    public Vector2[] SpawnPoint { get; }

    public LevelProperties(Tilemap map)
    {
        ArgumentNullException.ThrowIfNull(map);

        Options = EngineOptions.ParseFromTilemap(map);

        if (map.Layers.GetLayer<TilemapObjectLayer>(Constants.LDtkLayerDataName) is not { } dataLayer)
        {
            throw new ContentLoadException(
                $"The layer {Constants.LDtkLayerDataName} is required when loading an LDtk map.");
        }

        if (dataLayer.GetObjects<TilemapObject>() is not { } sps)
            throw new ContentLoadException($"Spawn point is required in {Constants.LDtkLayerDataName} layer.");

        SpawnPoint = [.. sps.Select(x => x.Position)];
    }
}