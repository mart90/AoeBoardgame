using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class StableFactory : PlaceableObjectFactory, ICanMakeUnitsFactory, ICanMakeResearchFactory
    {
        private int _hitPoints;

        public List<Type> UnitTypesAllowedToMake { get; set; }
        public List<ResearchEnum> ResearchAllowedToMake { get; set; }

        public StableFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Stable);
            TurnsToComplete = 2;
        }

        public override PlaceableObject Get(Player player)
        {
            return new Stable(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                LineOfSight = 1,
                UnitTypesAllowedToMake = new List<Type>(UnitTypesAllowedToMake),
                ResearchAllowedToMake = new List<ResearchEnum>(ResearchAllowedToMake),
                MeleeArmor = 2,
                RangedArmor = 5
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Stable";
            UiDescription = "Cavalry production building";

            _hitPoints = 70;

            UnitTypesAllowedToMake = new List<Type>
            {
                typeof(Scout)
            };

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 100)
            };

            ResearchAllowedToMake = new List<ResearchEnum>();
        }
    }
}
