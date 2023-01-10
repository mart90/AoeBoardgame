using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class BarracksFactory : PlaceableObjectFactory, ICanMakeUnitsFactory, ICanMakeResearchFactory
    {
        private int _hitPoints;

        public List<Type> UnitTypesAllowedToMake { get; set; }
        public List<ResearchEnum> ResearchAllowedToMake { get; set; }

        public BarracksFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Barracks);
            TurnsToComplete = 2;
        }

        public override PlaceableObject Get(Player player)
        {
            return new Barracks(TextureLibrary, player)
            {
                UiName = UiName,
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
            UiName = "Barracks";
            UiDescription = "Infantry production building";

            _hitPoints = 70;

            UnitTypesAllowedToMake = new List<Type>
            {
                typeof(Swordsman)
            };

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 100)
            };

            ResearchAllowedToMake = new List<ResearchEnum>();
        }
    }
}
