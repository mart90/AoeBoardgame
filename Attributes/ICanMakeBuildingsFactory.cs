using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeBuildingsFactory
    {
        List<Type> BuildingTypesAllowedToMake { get; set; }
    }
}
