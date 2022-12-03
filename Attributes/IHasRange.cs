using System.Collections.Generic;

namespace AoeBoardgame
{
    interface IHasRange
    {
        int Range { get; set; }
        bool HasMinimumRange { get; set; }
        IEnumerable<Tile> RangeableTiles { get; set; }
    }
}
