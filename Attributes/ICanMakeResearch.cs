using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeResearch : IHasQueue
    {
        Research ResearchQueued { get; set; }
        List<ResearchEnum> ResearchAllowedToMake { get; set; }
    }

    static class ICanMakeResearchMethods
    {
        public static bool HasResearchQueued<T>(this T builder) where T : ICanMakeResearch
        {
            return builder.ResearchQueued != null;
        }
    }
}
