using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Blacksmith : PlayerObject, ICanMakeResearch
    {
        public IEnumerable<Research> AllowedResearch { get; set; }
        public int QueueTurnsLeft { get; set; }
        public Research ResearchQueued { get; set; }

        public Blacksmith(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
