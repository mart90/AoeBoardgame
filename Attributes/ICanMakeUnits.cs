using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeUnits : IHasQueue
    {
        List<Type> UnitTypesAllowedToMake { get; set; }
        Type UnitTypeQueued { get; set; }
        Tile WayPoint { get; set; }
    }

    static class ICanMakeUnitsMethods
    {
        public static bool HasUnitQueued<T>(this T builder) where T : ICanMakeUnits
        {
            return builder.UnitTypeQueued != null;
        }
    }
}
