using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeUnits
    {
        IEnumerable<Type> UnitTypesAllowedToMake { get; set; }
        Tile WayPoint { get; set; }

        void MakeUnit<T>(Tile destinationTile) where T : PlayerObject;
    }
}
