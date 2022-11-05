using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class VillagerFactory : PlaceableObjectFactory
    {
        private int _hitPoints;
        private int _speed;
        private int _attackDamage;
        private int _lineOfSight;
        private List<Type> _buildingTypesAllowedToMake;

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
            TurnsToComplete = 2;
        }

        protected override void SetBaseValues()
        {
            UiName = "Villager";
            UiDescription = "Gathers resources and builds buildings";

            _hitPoints = 50;
            _speed = 2;
            _attackDamage = 1;
            _lineOfSight = 3;
            _buildingTypesAllowedToMake = new List<Type>
            {
                typeof(LumberCamp),
                typeof(Mine),
                typeof(Barracks),
                typeof(Tower) //TODO remove after tests
            };

            Cost = new List<ResourceCollection> 
            { 
                new ResourceCollection(Resource.Food, 50)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Villager(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                Speed = _speed,
                LineOfSight = _lineOfSight,
                AttackDamage = _attackDamage,
                BuildingTypesAllowedToMake = _buildingTypesAllowedToMake,
                FoodConsumption = 1
            };
        }
    }
}
