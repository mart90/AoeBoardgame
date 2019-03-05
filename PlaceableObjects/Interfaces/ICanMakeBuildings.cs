using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeBuildings : IHasObjectQueue
    {
        List<Type> BuildingTypesAllowedToMake { get; set; }

        void MakeBuilding<T>(Tile destinationTile) where T : PlayerObject;
    }
}
