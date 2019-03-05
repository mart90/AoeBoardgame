using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class VillagerFactory : PlaceableObjectFactory
    {
        private int _hitPoints;
        private int _speed;
        private int _attackDamage;
        private List<Type> _buildingTypesAllowedToMake;

        public const int FeudalAddedHitPoints = 20;
        public const int CastleAddedHitPoints = 20;
        public const int ImperialAddedHitPoints = 20;

        public const int FeudalAddedAttackDamage = 1;
        public const int CastleAddedAttackDamage = 2;
        public const int ImperialAddedAttackDamage = 3;

        public static readonly List<Type> FeudalAddedBuildings = new List<Type>
        {
            typeof(Stable),
            typeof(Tower),
            typeof(Farm),
            typeof(Blacksmith)
        };

        public static readonly List<Type> CastleAddedBuildings = new List<Type>
        {
            typeof(University),
            typeof(TownCenter),
            typeof(GuardTower),
            typeof(Castle),
            typeof(Church)
        };

        public VillagerFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Villager);
        }

        protected override void SetBaseStats()
        {
            _hitPoints = 50;
            _speed = 2;
            _attackDamage = 1;
            _buildingTypesAllowedToMake = new List<Type>
            {
                typeof(LumberCamp),
                typeof(Mine),
                typeof(Barracks)
            };

            Cost = new ResourceCollection(50);
        }

        public override PlaceableObject Get(Player player)
        {
            return new Villager(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                Speed = _speed,
                AttackDamage = _attackDamage,
                BuildingTypesAllowedToMake = _buildingTypesAllowedToMake
            };
        }

        public override void UpgradeToFeudalAge()
        {
            _hitPoints += FeudalAddedHitPoints;
            _attackDamage += FeudalAddedAttackDamage;
            _buildingTypesAllowedToMake.AddRange(FeudalAddedBuildings);
        }

        public override void UpgradeToCastleAge()
        {
            _hitPoints += CastleAddedHitPoints;
            _attackDamage += CastleAddedAttackDamage;
            _buildingTypesAllowedToMake.AddRange(CastleAddedBuildings);
        }

        public override void UpgradeToImperialAge()
        {
            _hitPoints += ImperialAddedHitPoints;
            _attackDamage += ImperialAddedAttackDamage;
        }
    }
}
