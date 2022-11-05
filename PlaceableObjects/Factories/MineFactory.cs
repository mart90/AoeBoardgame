using System.Collections.Generic;

namespace AoeBoardgame
{
    class MineFactory : PlaceableObjectFactory
    {
        private int _hitPoints;
        private int _lineOfSight;
        private int _maxGatherers;

        public MineFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Mine);
            TurnsToComplete = 1;
        }

        public override PlaceableObject Get(Player player)
        {
            return new Mine(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                LineOfSight = _lineOfSight,
                MaxGatherers = _maxGatherers
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Mine";
            UiDescription = "Gold, Iron and Stone gather building. Build on rocks";

            _hitPoints = 200;
            _lineOfSight = 1;
            _maxGatherers = 3;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 50)
            };
        }
    }
}
