using System.Collections.Generic;

namespace AoeBoardgame
{
    class LumberCamp : PlayerObject, IEconomicBuilding
    {
        public Resource Resource { get; set; }
        public int MaxGatherers { get; set; }
        public List<ICanGatherResources> Gatherers { get; private set; }

        public LumberCamp(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
            Gatherers = new List<ICanGatherResources>();
        }
    }
}
