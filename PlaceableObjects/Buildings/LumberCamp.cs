using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class LumberCamp : PlayerObject, IEconomicBuilding
    {
        public Resource Resource { get; set; }
        public int MaxUnits { get; set; }
        public List<ICanFormGroup> Units { get; }

        public override int LineOfSight
        {
            get => Units.Any() ? ((PlayerObject)Units[0]).LineOfSight : 1;
            set { }
        }

        public LumberCamp(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
            Units = new List<ICanFormGroup>();
        }
    }
}
