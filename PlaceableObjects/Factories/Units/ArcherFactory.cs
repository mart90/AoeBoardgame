using System.Collections.Generic;

namespace AoeBoardgame
{
    class ArcherFactory : PlaceableObjectFactory, ICanBeUpgraded, IMilitaryUnit, IArcher
    {
        public int HitPoints { get; set; }
        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public int Range { get; set; }
        public int ArmorPierce { get; set; }
        public int LineOfSight { get; set; }
        public int MeleeArmor { get; set; }
        public int RangedArmor { get; set; }
        public int UpgradeLevel { get; set; }

        public ArcherFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Archer);
            TurnsToComplete = 2;
            UpgradeLevel = 1;
        }

        protected override void SetBaseValues()
        {
            UiName = "Archer";
            UiDescription = "Has a ranged attack";

            HitPoints = 15;
            Speed = 2;
            AttackDamage = 4;
            Range = 2;
            LineOfSight = 3;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Food, 20),
                new ResourceCollection(Resource.Wood, 40),
                new ResourceCollection(Resource.Gold, 30)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Archer(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = Speed,
                LineOfSight = LineOfSight,
                AttackDamage = AttackDamage,
                Range = Range,
                ArmorPierce = ArmorPierce,
                GoldConsumption = 1,
                UpgradeLevel = UpgradeLevel,
                RangedArmor = RangedArmor,
                MeleeArmor = MeleeArmor
            };
        }
    }
}
