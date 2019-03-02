using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeBuildings
    {
        IEnumerable<PlaceableObjectType> BuildingTypesAllowedToMake { get; set; }
    }
}
