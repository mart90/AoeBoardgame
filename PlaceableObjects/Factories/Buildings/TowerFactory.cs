using System.Collections.Generic;

namespace AoeBoardgame
{
    class TowerFactory : PlaceableObjectFactory
    {
        public bool HasMinimumRange { get; set; }
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }

        private int _range;
        private int _lineOfSight;

        public TowerFactory(TextureLibrary textureLibrary) :
            base(textureLibrary)
        {
            Type = typeof(Tower);
            TurnsToComplete = 2;
            HasMinimumRange = true;
        }

        public override PlaceableObject Get(Player player) 
        {
            return new Tower(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                AttackDamage = AttackDamage,
                ArmorPierce = 2,
                Range = _range,
                HasMinimumRange = HasMinimumRange,
                LineOfSight = _lineOfSight,
                MeleeArmor = 1,
                RangedArmor = 3
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Tower";
            UiDescription = "Building with long line of sight and a ranged attack";

            HitPoints = 70;
            AttackDamage = 6;
            _range = 3;
            _lineOfSight = 4;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 50),
                new ResourceCollection(Resource.Stone, 60)
            };
        }
    }
}
