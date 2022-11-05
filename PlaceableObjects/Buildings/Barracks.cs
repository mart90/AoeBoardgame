using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Barracks : PlayerObject, 
        ICanMakeUnits,
        ICanMakeResearch
    {
        public IEnumerable<Type> UnitTypesAllowedToMake { get; set; }
        public Tile WayPoint { get; set; }
        public int QueueTurnsLeft { get; set; }
        public Type UnitTypeQueued { get; set; }
        public Research ResearchQueued { get; set; }
        public IEnumerable<Research> AllowedResearch { get; set; }

        public Barracks(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }

        public void MakeUnit<T>(Tile destinationTile) where T : PlayerObject
        {

        }
    }
}
