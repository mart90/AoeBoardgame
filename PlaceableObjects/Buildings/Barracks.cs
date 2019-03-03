using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Barracks : PlayerObject, ICanMakeUnits
    {
        public IEnumerable<Type> UnitTypesAllowedToMake { get; set; }

        public Barracks(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }

        public override void UpgradeToFeudalAge()
        {
            throw new NotImplementedException();
        }

        public override void UpgradeToCastleAge()
        {
            throw new NotImplementedException();
        }

        public override void UpgradeToImperialAge()
        {
            throw new NotImplementedException();
        }
    }
}
