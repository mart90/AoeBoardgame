using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeUnits
    {
        IEnumerable<PlaceableObjectType> UnitTypesAllowedToMake { get; set; }
    }
}
