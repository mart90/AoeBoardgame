using System.Collections.Generic;

namespace AoeBoardgame
{
    class GuardTower : PlayerObject,
        IAttacker,
        IHasRange
    {
        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }
        public bool HasAttackedThisTurn { get; set; }
        public int Range { get; set; }
        public IEnumerable<Tile> RangeableTiles { get; set; }
        public bool HasMinimumRange { get; set; }

        public GuardTower(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
