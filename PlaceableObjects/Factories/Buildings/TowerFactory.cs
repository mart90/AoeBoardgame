using System.Collections.Generic;

namespace AoeBoardgame
{
    class TowerFactory : PlaceableObjectFactory
    {
        public bool HasMinimumRange { get; set; }
        public int HitPoints { get; set; }

        private int _attackDamage;
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
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                AttackDamage = _attackDamage,
                ArmorPierce = 2,
                Range = _range,
                HasMinimumRange = HasMinimumRange,
                LineOfSight = _lineOfSight,
                MeleeArmor = 2,
                RangedArmor = 5
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Tower";
            UiDescription = "Building with long line of sight and a ranged attack";

            HitPoints = 60;
            _attackDamage = 3;
            _range = 3;
            _lineOfSight = 4;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 50),
                new ResourceCollection(Resource.Stone, 100)
            };
        }
    }
}
