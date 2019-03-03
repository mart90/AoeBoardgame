using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeBuildings
    {
        List<Type> BuildingTypesAllowedToMake { get; set; }
    }
}
