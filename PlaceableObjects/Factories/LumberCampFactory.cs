using System.Collections.Generic;

namespace AoeBoardgame
{
    class LumberCampFactory : PlaceableObjectFactory
    {
        private int _hitPoints;
        private int _lineOfSight;
        private int _maxGatherers;

        public LumberCampFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(LumberCamp);
            TurnsToComplete = 1;
        }

        public override PlaceableObject Get(Player player)
        {
            return new LumberCamp(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                LineOfSight = _lineOfSight,
                MaxGatherers = _maxGatherers
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Lumber camp";
            UiDescription = "Wood gather building. Build on trees";

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
