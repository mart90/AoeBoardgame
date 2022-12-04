using System.Collections.Generic;

namespace AoeBoardgame
{
    class CatapultFactory : PlaceableObjectFactory, IMilitaryUnit, ICanBeUpgraded
    {
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }
        public int RangedArmor { get; set; }
        public int MeleeArmor { get; set; }
        public int Range { get; set; }

        private int _speed;
        private int _lineOfSight;

        public int UpgradeLevel { get; set; }

        public CatapultFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Catapult);
            TurnsToComplete = 5;
            UpgradeLevel = 2;
        }

        protected override void SetBaseValues()
        {
            UiName = "Catapult";
            UiDescription = "Siege weapon. Good against infantry and buildings";

            HitPoints = 30;
            AttackDamage = 20;
            _speed = 1;
            Range = 3;
            _lineOfSight = 3;
            MeleeArmor = 0;
            RangedArmor = 4;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Wood, 150),
                new ResourceCollection(Resource.Gold, 50),
                new ResourceCollection(Resource.Iron, 100)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Catapult(TextureLibrary, player)
            {
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = _speed,
                LineOfSight = _lineOfSight,
                AttackDamage = AttackDamage,
                ArmorPierce = 10,
                Range = Range,
                UpgradeLevel = UpgradeLevel,
                HasMinimumRange = true,
                RangedArmor = RangedArmor,
                MeleeArmor = MeleeArmor
            };
        }
    }
}
