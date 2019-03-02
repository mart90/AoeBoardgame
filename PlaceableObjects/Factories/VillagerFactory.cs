using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class VillagerFactory : ObjectFactory
    {
        public override Type Type { get; }

        private int _hitPoints;
        private int _speed;
        private int _attackDamage;
        private IEnumerable<PlaceableObjectType> _buildingTypesAllowedToMake;

        public VillagerFactory(TextureLibrary textureLibrary, Player player)
            : base(textureLibrary, player)
        {
            Type = typeof(Villager);
        }

        public Villager Get()
        {
            return new Villager(TextureLibrary, Player)
            {
                HitPoints = _hitPoints,
                Speed = _speed,
                AttackDamage = _attackDamage,
            };
        }

        protected override void SetDefaults()
        {
            _hitPoints = 50;
            _speed = 2;
            _attackDamage = 10;
            _buildingTypesAllowedToMake = new List<PlaceableObjectType>
            {
                //TODO
            };
        }
    }
}
