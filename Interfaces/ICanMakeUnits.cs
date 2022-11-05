using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeUnits : IHasQueue
    {
        IEnumerable<Type> UnitTypesAllowedToMake { get; set; }
        Type UnitTypeQueued { get; set; }
        Tile WayPoint { get; set; }
    }
}
