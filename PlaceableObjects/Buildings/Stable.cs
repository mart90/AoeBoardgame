using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Stable : PlayerObject,
        ICanMakeUnits,
        ICanMakeResearch
    {
        public List<Type> UnitTypesAllowedToMake { get; set; }
        public Tile WayPoint { get; set; }
        public int QueueTurnsLeft { get; set; }
        public Type UnitTypeQueued { get; set; }
        public Research ResearchQueued { get; set; }
        public List<ResearchEnum> ResearchAllowedToMake { get; set; }

        public Stable(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
