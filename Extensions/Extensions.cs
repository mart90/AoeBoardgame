﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    static class TileListExtensions
    {
        public static void Highlight(this IEnumerable<Tile> tiles, TileColor color)
        {
            foreach (var tile in tiles)
            {
                tile.SetTemporaryColor(color);
            }
        }
    }

    static class PlayerObjectListExtensions 
    {
        public static IEnumerable<T> FilterByType<T>(this IEnumerable<PlayerObject> list)
        {
            return list.Where(e => e is T).Cast<T>();
        }
    }
}
