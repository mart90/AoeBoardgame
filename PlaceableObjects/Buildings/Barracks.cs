using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Barracks : PlayerObject, ICanMakeUnits
    {
        public IEnumerable<Type> UnitTypesAllowedToMake { get; set; }
        public Tile WayPoint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Barracks(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }

        public void MakeUnit<T>(Tile destinationTile) where T : PlayerObject
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
