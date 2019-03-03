using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeUnits
    {
        IEnumerable<Type> UnitTypesAllowedToMake { get; set; }
    }
}
