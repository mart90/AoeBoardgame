﻿using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Villager : Unit,
        IAttacker,
        ICanMakeBuildings
    {
        public int AttackDamage { get; set; }
        public IEnumerable<PlaceableObjectType> BuildingTypesAllowedToMake { get; set; }

        public Villager(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, PlaceableObjectType.Villager, owner)
        {
        }

        public void Attack(IAttackable defender)
        {
            throw new NotImplementedException();
        }
    }
}
