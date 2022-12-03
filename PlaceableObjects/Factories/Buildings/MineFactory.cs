using System.Collections.Generic;

namespace AoeBoardgame
{
    class MineFactory : PlaceableObjectFactory
    {
        public int MaxUnits { get; set; }

        private int _hitPoints;

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
                MaxUnits = MaxUnits,
                LineOfSight = 1,
                MeleeArmor = 1,
                RangedArmor = 3
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Mine";
            UiDescription = "Gold, Iron and Stone gather building. Build on rocks";

            _hitPoints = 30;
            MaxUnits = 2;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 30)
            };
        }
    }
}
