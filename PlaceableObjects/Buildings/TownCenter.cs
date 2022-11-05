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
        public int Range { get; set; }
        public IEnumerable<Type> UnitTypesAllowedToMake { get; set; }
        public IEnumerable<Research> AllowedResearch { get; set; }
        public int QueueTurnsLeft { get; set; }
        public Tile WayPoint { get; set; }
        public Type UnitTypeQueued { get; set; }
        public Research ResearchQueued { get; set; }

        public TownCenter(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
