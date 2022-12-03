using System.Collections.Generic;

namespace AoeBoardgame
{
    class ScoutFactory : PlaceableObjectFactory, IMilitaryUnit, ICavalry
    {
        public int HitPoints { get; set; }
        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public int RangedArmor { get; set; }
        public int MeleeArmor { get; set; }

        private int _lineOfSight;

        public int UpgradeAgeLevel { get; set; }

        public ScoutFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Scout);
            TurnsToComplete = 3;
        }

        protected override void SetBaseValues()
        {
            UiName = "Scout";
            UiDescription = "Cavalry with increased movement points and line of sight";

            HitPoints = 30;
            Speed = 4;
            AttackDamage = 3;
            _lineOfSight = 4;
            MeleeArmor = 0;
            RangedArmor = 0;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Food, 50),
                new ResourceCollection(Resource.Gold, 20)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Scout(TextureLibrary, player)
            {
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = Speed,
                LineOfSight = _lineOfSight,
                AttackDamage = AttackDamage,
                FoodConsumption = 2,
                RangedArmor = RangedArmor,
                MeleeArmor = MeleeArmor
            };
        }
    }
}
