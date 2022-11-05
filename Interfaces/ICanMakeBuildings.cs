using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeBuildings : IHasQueue
    {
        List<Type> BuildingTypesAllowedToMake { get; set; }
        Type BuildingTypeQueued { get; set; }
        Tile BuildingDestinationTile { get; set; }
    }
}
