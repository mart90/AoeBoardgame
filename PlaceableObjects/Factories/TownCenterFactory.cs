using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenterFactory : PlaceableObjectFactory
    {
        public override Type Type { get; }
        public override ResourceCollection Cost { get; protected set; }

        private int _hitPoints;
        private int _attackDamage;
        private int _range;
        private int _maxUnitsGarrisoned;
        private List<PlaceableObjectType> _unitTypesAllowedToMake;

        public TownCenterFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(TownCenter);
        }

        public override PlaceableObject Get(Player player) 
        {
            return new TownCenter(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                AttackDamage = _attackDamage,
                Range = _range,
                MaxUnitsGarrisoned = _maxUnitsGarrisoned,
                UnitTypesAllowedToMake = _unitTypesAllowedToMake
            };
        }

        protected override void SetDefaults()
        {
            _hitPoints = 1000;
            _attackDamage = 10;
            _range = 2;
            _maxUnitsGarrisoned = 3;

            _unitTypesAllowedToMake = new List<PlaceableObjectType>
            {
                PlaceableObjectType.Villager
            };

            Cost = new ResourceCollection(0, 100, 0, 0, 50);
        }

        public override void AdvanceToFeudalAge()
        {
            _hitPoints += 500;
            _attackDamage += 10;
            _range += 1;
        }

        public override void AdvanceToCastleAge()
        {
            _hitPoints += 1000;
            _attackDamage += 10;
            _range += 1;
        }

        public override void AdvanceToImperialAge()
        {
            _hitPoints += 1000;
            _attackDamage += 10;
            _range += 1;
        }
    }
}
