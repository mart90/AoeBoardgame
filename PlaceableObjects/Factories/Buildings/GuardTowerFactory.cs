using System.Collections.Generic;

namespace AoeBoardgame
{
    class GuardTowerFactory : PlaceableObjectFactory
    {
        public bool HasMinimumRange { get; set; }
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }

        private int _range;
        private int _lineOfSight;

        public GuardTowerFactory(TextureLibrary textureLibrary) :
            base(textureLibrary)
        {
            Type = typeof(GuardTower);
            TurnsToComplete = 3;
            HasMinimumRange = true;
        }

        public override PlaceableObject Get(Player player) 
        {
            return new GuardTower(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                AttackDamage = AttackDamage,
                ArmorPierce = 5,
                Range = _range,
                HasMinimumRange = HasMinimumRange,
                LineOfSight = _lineOfSight,
                MeleeArmor = 2,
                RangedArmor = 5
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Guard\ntower";
            UiDescription = "Building with long line of sight and a ranged attack";

            HitPoints = 100;
            AttackDamage = 12;
            _range = 4;
            _lineOfSight = 5;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 100),
                new ResourceCollection(Resource.Stone, 100)
            };
        }
    }
}
