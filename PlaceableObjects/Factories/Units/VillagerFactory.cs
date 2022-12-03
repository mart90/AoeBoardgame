using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class VillagerFactory : PlaceableObjectFactory, ICanMakeBuildingsFactory
    {
        public int HitPoints { get; set; }
        public int Speed { get; set; }

        private int _attackDamage;
        private int _lineOfSight;
        private int _meleeArmor;
        private int _rangedArmor;

        public List<Type> BuildingTypesAllowedToMake { get; set; }

        public VillagerFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Villager);
            TurnsToComplete = 3;
        }

        protected override void SetBaseValues()
        {
            UiName = "Villager";
            UiDescription = "Gathers resources and builds buildings";

            HitPoints = 10;
            Speed = 2;
            _attackDamage = 1;
            _lineOfSight = 3;
            _meleeArmor = 0;
            _rangedArmor = 0;

            BuildingTypesAllowedToMake = new List<Type>
            {
                typeof(LumberCamp),
                typeof(Farm),
                typeof(Mine),
                typeof(Barracks)
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
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = Speed,
                LineOfSight = _lineOfSight,
                AttackDamage = _attackDamage,
                BuildingTypesAllowedToMake = new List<Type>(BuildingTypesAllowedToMake),
                FoodConsumption = 1,
                RangedArmor = _rangedArmor,
                MeleeArmor = _meleeArmor
            };
        }
    }
}
