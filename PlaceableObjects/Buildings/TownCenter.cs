using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenter : PlayerObject, 
        IAttacker,
        IHasRange,
        IGarrisonable, 
        ICanMakeUnits, 
        ICanMakeResearch
    {
        public int AttackDamage { get; set; }
        public int Range { get; set; }
        public int MaxUnitsGarrisoned { get; set; }
        public IEnumerable<Type> UnitTypesAllowedToMake { get; set; }
        public IEnumerable<Research> AllowedResearch { get; set; }
        public QueuedObject QueuedObject { get; set; }
        public Tile WayPoint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TownCenter(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }

        public void MakeUnit<T>(Tile destinationTile) where T : PlayerObject
        {
            if (QueuedObject != null)
            {
                return;
            }

            QueuedObject = new QueuedObject
            {
                ObjectType = typeof(T),
                DestinationTile = destinationTile
            };
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
