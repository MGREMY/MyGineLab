using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps;

namespace FirstGame.GameObjects;

public class GameLevelData
{
    public required Vector2 SpawnPoint { get; init; }
}

public class GameLevel
{
    public required Tilemap Tilemap { get; set; }
    public required GameLevelData Data { get; set; }

    public static GameLevel FromTilemap(Tilemap tilemap)
    {
        var data = ParseData(tilemap);

        return new GameLevel
        {
            Tilemap = tilemap,
            Data = data,
        };
    }

    private static GameLevelData ParseData(Tilemap tilemap)
    {
        if (tilemap.Layers["MapData"] is not TilemapObjectLayer mapData)
            throw new NullReferenceException("level must have a 'MapData' layer");

        return new GameLevelData
        {
            SpawnPoint = mapData.Objects.First(x => x.Name is "SpawnPoint").Position,
        };
    }
}