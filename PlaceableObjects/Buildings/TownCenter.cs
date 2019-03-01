using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenter : Building, 
        ICanAttack,
        ICanBeGarrisoned, 
        ICanMakeUnits, 
        ICanMakeResearch, 
        IHasRange
    {
        public int AttackDamage { get; set; }
        public int Range { get; set; }
        public int MaxUnitsGarrisoned { get; set; }
        public IEnumerable<Type> UnitTypesAllowedToMake { get; set; }

        public TownCenter(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, PlaceableObjectType.TownCenter, owner)
        {
        }

        public void Attack(ICanBeAttacked defender)
        {
            throw new NotImplementedException();
        }
    }
}
