using System.Collections.Generic;

namespace AoeBoardgame
{
    class FarmFactory : PlaceableObjectFactory
    {
        public int MaxUnits { get; set; }

        private int _hitPoints;

        public FarmFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Farm);
            TurnsToComplete = 1;
        }

        public override PlaceableObject Get(Player player)
        {
            return new Farm(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                MaxUnits = MaxUnits,
                LineOfSight = 1,
                Resource = Resource.Food,
                MeleeArmor = 0,
                RangedArmor = 0
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Farm";
            UiDescription = "Food gather building";

            _hitPoints = 8;
            MaxUnits = 1;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 70)
            };
        }
    }
}
