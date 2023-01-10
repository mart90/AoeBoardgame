using System.Collections.Generic;

namespace AoeBoardgame
{
    class SwordsmanFactory : PlaceableObjectFactory, ICanBeUpgraded, IMilitaryUnit, IInfantry
    {
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }
        public int RangedArmor { get; set; }
        public int MeleeArmor { get; set; }

        private int _speed;
        private int _lineOfSight;

        public int UpgradeLevel { get; set; }

        public SwordsmanFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Swordsman);
            TurnsToComplete = 2;
            UpgradeLevel = 0;
        }

        protected override void SetBaseValues()
        {
            UiName = "Swordsman";
            UiDescription = "Basic infantry unit";

            HitPoints = 20;
            _speed = 2;
            AttackDamage = 3;
            _lineOfSight = 3;
            MeleeArmor = 1;
            RangedArmor = 1;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Food, 30),
                new ResourceCollection(Resource.Gold, 20),
                new ResourceCollection(Resource.Iron, 20)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Swordsman(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = _speed,
                LineOfSight = _lineOfSight,
                AttackDamage = AttackDamage,
                FoodConsumption = 1,
                UpgradeLevel = UpgradeLevel,
                RangedArmor = RangedArmor,
                MeleeArmor = MeleeArmor
            };
        }
    }
}
