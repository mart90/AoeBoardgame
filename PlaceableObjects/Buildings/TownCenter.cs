using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenter : PlayerObject, 
        IAttacker,
        IGarrisonable, 
        ICanMakeUnits, 
        ICanMakeResearch, 
        IHasRange
    {
        public int AttackDamage { get; set; }
        public int Range { get; set; }
        public int MaxUnitsGarrisoned { get; set; }
        public IEnumerable<Type> UnitTypesAllowedToMake { get; set; }
        public IEnumerable<Research> AllowedResearch { get; set; }

        public TownCenter(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }

        public void Attack(IAttackable defender)
        {
            throw new NotImplementedException();
        }

        public override void UpgradeToFeudalAge()
        {
            AddHitPoints(TownCenterFactory.FeudalAddedHitPoints);
            AttackDamage += TownCenterFactory.FeudalAddedAttackDamage;
        }

        public override void UpgradeToCastleAge()
        {
            AddHitPoints(TownCenterFactory.CastleAddedHitPoints);
            AttackDamage += TownCenterFactory.CastleAddedAttackDamage;
        }

        public override void UpgradeToImperialAge()
        {
            AddHitPoints(TownCenterFactory.ImperialAddedHitPoints);
            AttackDamage += TownCenterFactory.ImperialAddedAttackDamage;
        }
    }
}
