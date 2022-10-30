using System.Collections.Generic;

namespace AoeBoardgame.Extensions
{
    static class Extensions
    {
        public static void Highlight(this IEnumerable<Tile> tiles, TileColor color)
        {
            foreach (var tile in tiles)
            {
                tile.SetTemporaryColor(color);
            }
        }
    }
}
