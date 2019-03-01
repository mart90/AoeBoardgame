using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeBuildings
    {
        IEnumerable<Type> BuildingTypesAllowedToMake { get; set; }
    }
}
