using System;
using System.Collections.Generic;
using MonoGame.Extended.Tilemaps;

namespace Platformer.Extensions;

public static class TilemapTileLayerExtensions
{
    extension(TilemapTileLayer tileLayer)
    {
        public int[,] GetTileGrid()
        {
            var grid = new int[tileLayer.Width, tileLayer.Height];

            foreach (var entry in tileLayer.GetTiles())
                grid[entry.X, entry.Y] = 1;

            return grid;
        }

        public IEnumerable<(int x, int y, int width, int height)> GetTileMerged(int[,]? solid = null)
        {
            solid ??= tileLayer.GetTileGrid();

            var width = solid.GetLength(0);
            var height = solid.GetLength(1);

            // active maps "horizontal run key (start, end)" -> the row where that
            // run first appeared (the top edge of the growing rectangle).
            var active = new Dictionary<(int start, int end), int>();

            // One pass per row, plus one sentinel pass (y == height) where the row
            // is empty on purpose. That final pass has "no run continues here", so
            // every remaining active rectangle is flushed.
            for (var y = 0; y <= height; y++)
            {
                var next = new Dictionary<(int, int), int>();

                // Build the horizontal runs of this row: consecutive solid cells
                // with the same "class" (int-grid value) form a single run.
                // Key is (start, x) with x exclusive — a half-open interval.
                if (y < height)
                {
                    for (var x = 0; x < width;)
                    {
                        if (solid[x, y] == 0)
                        {
                            x++;
                            continue;
                        }

                        var start = x;
                        var cls = solid[start, y];
                        // merge only equal class ids
                        while (x < width && solid[x, y] == cls) x++;

                        // candidate: run starts a rectangle at THIS row...
                        next[(start, x)] = y;
                    }
                }

                // ...unless it's a continuation of a run from the row above. If the
                // same (start,end) existed in the active row, the rectangle did NOT
                // end here, so keep its original top edge instead of the current y.
                foreach (var kvp in next)
                    if (active.TryGetValue(kvp.Key, out var yStart))
                        next[kvp.Key] = yStart;

                // Any run from the previous row that did NOT continue into this one
                // is a finished rectangle: top = its yStart, height = y - yStart
                // (y acts as the exclusive bottom edge). Emit and forget it.
                foreach (var kvp in active)
                    if (!next.ContainsKey(kvp.Key))
                        yield return new ValueTuple<int, int, int, int>(
                            kvp.Key.start,
                            kvp.Value,
                            kvp.Key.end - kvp.Key.start,
                            y - kvp.Value);

                active = next;
            }
        }
    }
}