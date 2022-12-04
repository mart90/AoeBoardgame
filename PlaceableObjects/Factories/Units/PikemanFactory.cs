using System.Collections.Generic;

namespace AoeBoardgame
{
    class PikemanFactory : PlaceableObjectFactory, ICanBeUpgraded, IMilitaryUnit, IInfantry
    {
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }
        public int RangedArmor { get; set; }
        public int MeleeArmor { get; set; }

        private int _speed;
        private int _lineOfSight;

        public int UpgradeLevel { get; set; }

        public PikemanFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Pikeman);
            TurnsToComplete = 3;
            UpgradeLevel = 1;
        }

        protected override void SetBaseValues()
        {
            UiName = "Pikeman";
            UiDescription = "Anti-cavalry unit";

            HitPoints = 40;
            _speed = 2;
            AttackDamage = 3;
            _lineOfSight = 3;
            MeleeArmor = 0;
            RangedArmor = 0;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Food, 40),
                new ResourceCollection(Resource.Gold, 20),
                new ResourceCollection(Resource.Iron, 20)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Pikeman(TextureLibrary, player)
            {
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = _speed,
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
