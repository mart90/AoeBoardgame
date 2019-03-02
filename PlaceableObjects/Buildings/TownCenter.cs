using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenter : Building, 
        IAttacker,
        IGarrisonable, 
        ICanMakeUnits, 
        ICanMakeResearch, 
        IHasRange
    {
        public int AttackDamage { get; set; }
        public int Range { get; set; }
        public int MaxUnitsGarrisoned { get; set; }
        public IEnumerable<PlaceableObjectType> UnitTypesAllowedToMake { get; set; }

        public TownCenter(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, PlaceableObjectType.TownCenter, owner)
        {

        }

        public void Attack(IAttackable defender)
        {
            throw new NotImplementedException();
        }
    }
}
