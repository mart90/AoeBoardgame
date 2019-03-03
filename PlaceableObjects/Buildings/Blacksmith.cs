using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Blacksmith : PlayerObject, ICanMakeResearch
    {
        public IEnumerable<Research> AllowedResearch { get; set; }

        public Blacksmith(TextureLibrary textureLibrary, Player owner) :
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
