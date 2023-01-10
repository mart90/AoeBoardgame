using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeBuildings : IHasQueue
    {
        List<Type> BuildingTypesAllowedToMake { get; set; }
        Type BuildingTypeQueued { get; set; }
        Tile BuildingDestinationTile { get; set; }
    }

    static class ICanMakeBuildingsMethods
    {
        public static bool HasBuildingQueued<T>(this T builder) where T : ICanMakeBuildings
        {
            return builder.BuildingTypeQueued != null;
        }

        public static void StopConstruction<T>(this T builder) where T : ICanMakeBuildings
        {
            builder.BuildingTypeQueued = null;
            builder.QueueTurnsLeft = 0;
            builder.BuildingDestinationTile.BuildingUnderConstruction = false;
            builder.BuildingDestinationTile = null;
        }
    }
}
