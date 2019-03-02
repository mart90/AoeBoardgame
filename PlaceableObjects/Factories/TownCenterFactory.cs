using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenterFactory : ObjectFactory
    {
        public override Type Type { get; }

        private int _hitPoints;
        private int _attackDamage;
        private int _range;
        private int _maxUnitsGarrisoned;
        private IEnumerable<PlaceableObjectType> _unitTypesAllowedToMake;

        public TownCenterFactory(TextureLibrary textureLibrary, Player player)
            : base(textureLibrary, player)
        {
            Type = typeof(TownCenter);
        }

        public TownCenter Get()
        {
            return new TownCenter(TextureLibrary, Player)
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
            _range = 3;
            _maxUnitsGarrisoned = 5;

            _unitTypesAllowedToMake = new List<PlaceableObjectType>
            {
                PlaceableObjectType.Villager
            };
        }

        public void SetDefaultsForFeudalAge()
        {
            _hitPoints = 1500;
            _attackDamage = 20;
            _range = 4;
            _maxUnitsGarrisoned = 10;
        }
    }
}
