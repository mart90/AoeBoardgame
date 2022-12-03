using System.Collections.Generic;

namespace AoeBoardgame
{
    class KnightFactory : PlaceableObjectFactory, ICanBeUpgraded, IMilitaryUnit, ICavalry
    {
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }
        public int MeleeArmor { get; set; }
        public int RangedArmor { get; set; }
        public int Speed { get; set; }

        private int _lineOfSight;

        public int UpgradeLevel { get; set; }

        public KnightFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Knight);
            TurnsToComplete = 4;
            UpgradeLevel = 2;
        }

        protected override void SetBaseValues()
        {
            UiName = "Knight";
            UiDescription = "Armored cavalry";

            HitPoints = 70;
            Speed = 4;
            AttackDamage = 8;
            _lineOfSight = 3;
            MeleeArmor = 2;
            RangedArmor = 3;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Food, 40),
                new ResourceCollection(Resource.Gold, 30),
                new ResourceCollection(Resource.Iron, 60)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Knight(TextureLibrary, player)
            {
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = Speed,
                LineOfSight = _lineOfSight,
                AttackDamage = AttackDamage,
                FoodConsumption = 2,
                UpgradeLevel = UpgradeLevel,
                RangedArmor = RangedArmor,
                MeleeArmor = MeleeArmor
            };
        }
    }
}
