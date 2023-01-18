using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class VillagerFactory : PlaceableObjectFactory, ICanMakeBuildingsFactory
    {
        public int HitPoints { get; set; }
        public int Speed { get; set; }
        public int AttackDamage { get; set; }

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
            AttackDamage = 1;
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
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = Speed,
                LineOfSight = _lineOfSight,
                AttackDamage = AttackDamage,
                BuildingTypesAllowedToMake = new List<Type>(BuildingTypesAllowedToMake),
                RangedArmor = _rangedArmor,
                MeleeArmor = _meleeArmor
            };
        }
    }
}
