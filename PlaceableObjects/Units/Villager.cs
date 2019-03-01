using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Villager : Unit,
        ICanMove,
        ICanAttack,
        ICanMakeBuildings
    {
        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public IEnumerable<Type> BuildingTypesAllowedToMake { get; set; }

        public Villager(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, PlaceableObjectType.Villager, owner)
        {
        }

        public void Attack(ICanBeAttacked defender)
        {
            throw new NotImplementedException();
        }
    }
}
