using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenterFactory : PlaceableObjectFactory
    {
        private int _hitPoints;
        private int _armor;
        private int _attackDamage;
        private int _range;
        private int _lineOfSight;
        private List<Type> _unitTypesAllowedToMake;

        public TownCenterFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(TownCenter);
            TurnsToComplete = 4;
        }

        public override PlaceableObject Get(Player player) 
        {
            return new TownCenter(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                AttackDamage = _attackDamage,
                Armor = _armor,
                Range = _range,
                LineOfSight = _lineOfSight,
                UnitTypesAllowedToMake = _unitTypesAllowedToMake
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Town center";
            UiDescription = "Economic units and research";

            _hitPoints = 1000;
            _attackDamage = 10;
            _range = 3;
            _armor = 10;
            _lineOfSight = 4;

            _unitTypesAllowedToMake = new List<Type>
            {
                typeof(Villager)
            };

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 150),
                new ResourceCollection(Resource.Stone, 100)
            };
        }
    }
}
