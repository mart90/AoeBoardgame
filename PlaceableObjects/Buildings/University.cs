using System.Collections.Generic;

namespace AoeBoardgame
{
    class University : PlayerObject, ICanMakeResearch
    {
        public List<ResearchEnum> ResearchAllowedToMake { get; set; }
        public int QueueTurnsLeft { get; set; }
        public Research ResearchQueued { get; set; }

        public University(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
