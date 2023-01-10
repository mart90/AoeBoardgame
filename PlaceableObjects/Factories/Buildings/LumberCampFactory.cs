using System.Collections.Generic;

namespace AoeBoardgame
{
    class LumberCampFactory : PlaceableObjectFactory
    {
        public int MaxUnits { get; set; }

        private int _hitPoints;

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
                UiName = UiName,
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                MaxUnits = MaxUnits,
                LineOfSight = 1,
                Resource = Resource.Wood,
                MeleeArmor = 1,
                RangedArmor = 3
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Lumber camp";
            UiDescription = "Wood gather building. Build on trees";

            _hitPoints = 30;
            MaxUnits = 2;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 50)
            };
        }
    }
}
