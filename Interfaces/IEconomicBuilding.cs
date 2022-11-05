using System.Collections.Generic;

namespace AoeBoardgame
{
    interface IEconomicBuilding
    {
        Resource Resource { get; set; }
        List<ICanGatherResources> Gatherers { get; }
        int MaxGatherers { get; set; }
    }
}
