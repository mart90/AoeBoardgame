using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenterFactory : PlaceableObjectFactory
    {
        private int _hitPoints;
        private int _attackDamage;
        private int _range;
        private int _lineOfSight;
        private int _maxUnitsGarrisoned;
        private List<Type> _unitTypesAllowedToMake;

        public const int FeudalAddedHitPoints = 500;
        public const int CastleAddedHitPoints = 1000;
        public const int ImperialAddedHitPoints = 1000;

        public const int FeudalAddedAttackDamage = 10;
        public const int CastleAddedAttackDamage = 20;
        public const int ImperialAddedAttackDamage = 30;

        public TownCenterFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(TownCenter);
        }

        public override PlaceableObject Get(Player player) 
        {
            return new TownCenter(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                AttackDamage = _attackDamage,
                Range = _range,
                LineOfSight = _lineOfSight,
                MaxUnitsGarrisoned = _maxUnitsGarrisoned,
                UnitTypesAllowedToMake = _unitTypesAllowedToMake
            };
        }

        protected override void SetBaseStats()
        {
            _hitPoints = 1000;
            _attackDamage = 10;
            _range = 3;
            _lineOfSight = 4;
            _maxUnitsGarrisoned = 3;

            _unitTypesAllowedToMake = new List<Type>
            {
                typeof(Villager)
            };

            Cost = new ResourceCollection(0, 150, 0, 0, 50);
        }

        public override void UpgradeToFeudalAge()
        {
            _hitPoints += FeudalAddedHitPoints;
            _attackDamage += FeudalAddedAttackDamage;
        }

        public override void UpgradeToCastleAge()
        {
            _hitPoints += CastleAddedHitPoints;
            _attackDamage += CastleAddedAttackDamage;
        }

        public override void UpgradeToImperialAge()
        {
            _hitPoints += ImperialAddedHitPoints;
            _attackDamage += ImperialAddedAttackDamage;
        }
    }
}
