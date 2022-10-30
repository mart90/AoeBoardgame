using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeBuildings
    {
        List<Type> BuildingTypesAllowedToMake { get; set; }

        void MakeBuilding<T>(Tile destinationTile) where T : PlayerObject;
    }
}
