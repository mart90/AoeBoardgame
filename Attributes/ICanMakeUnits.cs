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
        public static bool HasUnitQueued<T>(this T trainer) where T : ICanMakeUnits
        {
            return trainer.UnitTypeQueued != null;
        }

        public static void StopQueue<T>(this T trainer) where T : ICanMakeUnits
        {
            trainer.UnitTypeQueued = null;
            trainer.QueueTurnsLeft = 0;
        }
    }
}
