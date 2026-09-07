using System.Linq;
using MonoGame.Extended.Tilemaps;

namespace FirstGame.GameObjects;

public class GameWorldData
{
}

public class GameWorld
{
    public required TilemapWorld TilemapWorld { get; set; }
    public required GameWorldData Data { get; set; }
    public required GameLevel CurrentGameLevel { get; set; }
    public required GameLevel[] Levels { get; set; }

    public static GameWorld FromTilemapWorld(TilemapWorld tilemapWorld)
    {
        var data = ParseData(tilemapWorld);
        var levels = tilemapWorld.Levels.Select(GameLevel.FromTilemap).ToArray();

        return new GameWorld
        {
            TilemapWorld = tilemapWorld,
            Data = data,
            Levels = levels,
            CurrentGameLevel = levels[0],
        };
    }

    private static GameWorldData ParseData(TilemapWorld tilemapWorld)
    {
        return new GameWorldData();
    }
}