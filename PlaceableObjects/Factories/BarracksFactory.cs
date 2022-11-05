using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class BarracksFactory : PlaceableObjectFactory
    {
        private int _hitPoints;
        private int _lineOfSight;
        private List<Type> _unitTypesAllowedToMake;

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
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                LineOfSight = _lineOfSight,
                UnitTypesAllowedToMake = _unitTypesAllowedToMake
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Barracks";
            UiDescription = "Infantry and archery units";

            _hitPoints = 500;
            _lineOfSight = 1;

            _unitTypesAllowedToMake = new List<Type>
            {
                // TODO
            };

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 100)
            };
        }
    }
}
