using System.Collections.Generic;

namespace AoeBoardgame
{
    class ThrowingAxemanFactory : PlaceableObjectFactory, ICanBeUpgraded, IMilitaryUnit, IInfantry
    {
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }
        public int RangedArmor { get; set; }
        public int MeleeArmor { get; set; }
        public int ArmorPierce { get; set; }

        private int _speed;
        private int _range;
        private int _lineOfSight;

        public int UpgradeLevel { get; set; }

        public ThrowingAxemanFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(ThrowingAxeman);
            TurnsToComplete = 2;
            UpgradeLevel = 2;
        }

        protected override void SetBaseValues()
        {
            UiName = "Throwing axeman";
            UiDescription = "Ranged unit with an armor-piercing attack";

            HitPoints = 35;
            _speed = 2;
            AttackDamage = 7;
            ArmorPierce = 3;
            _range = 2;
            _lineOfSight = 3;
            MeleeArmor = 0;
            RangedArmor = 0;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Food, 60),
                new ResourceCollection(Resource.Gold, 30),
                new ResourceCollection(Resource.Iron, 40)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new ThrowingAxeman(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = _speed,
                LineOfSight = _lineOfSight,
                AttackDamage = AttackDamage,
                ArmorPierce = ArmorPierce,
                Range = _range,
                FoodConsumption = 1,
                UpgradeLevel = UpgradeLevel,
                RangedArmor = RangedArmor,
                MeleeArmor = MeleeArmor
            };
        }
    }
}
