using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeUnitsFactory
    {
        List<Type> UnitTypesAllowedToMake { get; set; }
    }
}
