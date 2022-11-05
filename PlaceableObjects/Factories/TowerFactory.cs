using System.Collections.Generic;

namespace AoeBoardgame
{
    class TowerFactory : PlaceableObjectFactory
    {
        private int _hitPoints;
        private int _attackDamage;
        private int _range;
        private int _lineOfSight;

        public TowerFactory(TextureLibrary textureLibrary) :
            base(textureLibrary)
        {
            Type = typeof(Tower);
            TurnsToComplete = 3;
        }

        public override PlaceableObject Get(Player player) 
        {
            return new Tower(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                AttackDamage = _attackDamage,
                Range = _range,
                LineOfSight = _lineOfSight,
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Tower";
            UiDescription = "Long line of sight and a ranged attack";

            _hitPoints = 200;
            _attackDamage = 20;
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
