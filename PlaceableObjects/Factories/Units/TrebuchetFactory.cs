using System.Collections.Generic;

namespace AoeBoardgame
{
    class TrebuchetFactory : PlaceableObjectFactory, IMilitaryUnit
    {
        public int Range { get; set; }
        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }
        public int RangedArmor { get; set; }
        public int MeleeArmor { get; set; }

        private int _hitPoints;
        private int _speed;
        private int _lineOfSight;

        public TrebuchetFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Trebuchet);
            TurnsToComplete = 4;
        }

        protected override void SetBaseValues()
        {
            UiName = "Trebuchet";
            UiDescription = "Siege weapon. Can only attack buildings";

            _hitPoints = 40;
            _speed = 1;
            AttackDamage = 80;
            ArmorPierce = 100;
            Range = 5;
            _lineOfSight = 5;
            MeleeArmor = 0;
            RangedArmor = 4;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Wood, 200),
                new ResourceCollection(Resource.Gold, 50),
                new ResourceCollection(Resource.Iron, 100)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Trebuchet(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                Speed = _speed,
                LineOfSight = _lineOfSight,
                AttackDamage = AttackDamage,
                ArmorPierce = ArmorPierce,
                Range = Range,
                HasMinimumRange = true,
                RangedArmor = RangedArmor,
                MeleeArmor = MeleeArmor
            };
        }
    }
}
