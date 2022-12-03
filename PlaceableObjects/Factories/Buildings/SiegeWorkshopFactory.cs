using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class SiegeWorkshopFactory : PlaceableObjectFactory, ICanMakeUnitsFactory, ICanMakeResearchFactory
    {
        private int _hitPoints;

        public List<Type> UnitTypesAllowedToMake { get; set; }
        public List<ResearchEnum> ResearchAllowedToMake { get; set; }

        public SiegeWorkshopFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(SiegeWorkshop);
            TurnsToComplete = 2;
        }

        public override PlaceableObject Get(Player player)
        {
            return new SiegeWorkshop(TextureLibrary, player)
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
            UiName = "Siege workshop";
            UiDescription = "Siege weapon production building";

            _hitPoints = 100;

            UnitTypesAllowedToMake = new List<Type>
            {
                typeof(Catapult)
            };

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 200)
            };

            ResearchAllowedToMake = new List<ResearchEnum>();
        }
    }
}
