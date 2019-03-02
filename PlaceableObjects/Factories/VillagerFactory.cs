using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class VillagerFactory : PlaceableObjectFactory
    {
        public override Type Type { get; }
        public override ResourceCollection Cost { get; protected set; }

        private int _hitPoints;
        private int _speed;
        private int _attackDamage;
        private List<PlaceableObjectType> _buildingTypesAllowedToMake;
        
        public VillagerFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Villager);
        }

        public override PlaceableObject Get(Player player)
        {
            return new Villager(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                Speed = _speed,
                AttackDamage = _attackDamage,
                BuildingTypesAllowedToMake = _buildingTypesAllowedToMake
            };
        }

        protected override void SetDefaults()
        {
            _hitPoints = 50;
            _speed = 2;
            _attackDamage = 2;
            _buildingTypesAllowedToMake = new List<PlaceableObjectType>
            {
                PlaceableObjectType.LumberCamp,
                PlaceableObjectType.Mine,
                PlaceableObjectType.Barracks
            };

            Cost = new ResourceCollection(50);
        }

        public override void AdvanceToFeudalAge()
        {
            _hitPoints += 20;
            _attackDamage += 3;

            _buildingTypesAllowedToMake.AddRange(new List<PlaceableObjectType>
            {
                PlaceableObjectType.Stable,
                PlaceableObjectType.Tower,
                PlaceableObjectType.Farms,
                PlaceableObjectType.Blacksmith
            });
        }

        public override void AdvanceToCastleAge()
        {
            _hitPoints += 30;
            _attackDamage += 5;

            _buildingTypesAllowedToMake.AddRange(new List<PlaceableObjectType>
            {
                PlaceableObjectType.University,
                PlaceableObjectType.TownCenter,
                PlaceableObjectType.GuardTower,
                PlaceableObjectType.Castle,
                PlaceableObjectType.Church
            });
        }

        public override void AdvanceToImperialAge()
        {
            _hitPoints += 50;
            _attackDamage += 10;
        }
    }
}
