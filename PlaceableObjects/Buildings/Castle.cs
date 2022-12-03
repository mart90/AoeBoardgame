using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Castle : PlayerObject,
        ICanMakeUnits,
        IAttacker,
        IHasRange,
        ICanMakeResearch
    {
        public List<Type> UnitTypesAllowedToMake { get; set; }
        public Tile WayPoint { get; set; }
        public int QueueTurnsLeft { get; set; }
        public Type UnitTypeQueued { get; set; }
        public Research ResearchQueued { get; set; }
        public List<ResearchEnum> ResearchAllowedToMake { get; set; }

        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }
        public bool HasAttackedThisTurn { get; set; }

        public int Range { get; set; }
        public bool HasMinimumRange { get; set; }
        public IEnumerable<Tile> RangeableTiles { get; set; }

        public Castle(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
