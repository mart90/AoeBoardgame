using System.Collections.Generic;

namespace AoeBoardgame
{
    class LongbowmanFactory : PlaceableObjectFactory, ICanBeUpgraded, IMilitaryUnit, IArcher
    {
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }
        public int RangedArmor { get; set; }
        public int MeleeArmor { get; set; }
        
        private int _speed;
        private int _range;
        private int _lineOfSight;

        public int UpgradeLevel { get; set; }

        public LongbowmanFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Longbowman);
            TurnsToComplete = 2;
            UpgradeLevel = 2;
        }

        protected override void SetBaseValues()
        {
            UiName = "Longbowman";
            UiDescription = "Long-range archer";

            HitPoints = 30;
            _speed = 2;
            AttackDamage = 8;
            _range = 4;
            _lineOfSight = 4;
            MeleeArmor = 0;
            RangedArmor = 0;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Food, 40),
                new ResourceCollection(Resource.Wood, 50),
                new ResourceCollection(Resource.Gold, 30)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Longbowman(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = _speed,
                LineOfSight = _lineOfSight,
                AttackDamage = AttackDamage,
                Range = _range,
                FoodConsumption = 1,
                UpgradeLevel = UpgradeLevel,
                RangedArmor = RangedArmor,
                MeleeArmor = MeleeArmor
            };
        }
    }
}
