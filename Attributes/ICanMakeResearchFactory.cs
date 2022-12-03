using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeResearchFactory
    {
        List<ResearchEnum> ResearchAllowedToMake { get; set; }
    }
}
