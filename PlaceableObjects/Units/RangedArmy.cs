using System.Collections.Generic;

namespace AoeBoardgame
{
    class RangedArmy : Army, IHasRange
    {
        public int Range
        {
            get => ((IHasRange)Units[0]).Range;
            set { }
        }

        public bool HasMinimumRange { get; set; }
        public IEnumerable<Tile> RangeableTiles { get; set; }

        public RangedArmy(TextureLibrary textureLibrary, Player owner) : base(textureLibrary, owner)
        {
        }
    }
}
