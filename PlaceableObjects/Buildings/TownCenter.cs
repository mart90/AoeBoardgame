using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenter : PlayerObject, 
        IAttacker,
        IHasRange,
        ICanMakeUnits, 
        ICanMakeResearch
    {
        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }
        public bool HasAttackedThisTurn { get; set; }
        public int Range { get; set; }
        public List<Type> UnitTypesAllowedToMake { get; set; }
        public List<ResearchEnum> ResearchAllowedToMake { get; set; }
        public int QueueTurnsLeft { get; set; }
        public Tile WayPoint { get; set; }
        public Type UnitTypeQueued { get; set; }
        public Research ResearchQueued { get; set; }
        public IEnumerable<Tile> RangeableTiles { get; set; }
        public bool HasMinimumRange { get; set; }

        public TownCenter(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
