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
        public static bool HasResearchQueued<T>(this T researcher) where T : ICanMakeResearch
        {
            return researcher.ResearchQueued != null;
        }

        public static void StopQueue<T>(this T researcher) where T : ICanMakeResearch
        {
            researcher.ResearchQueued = null;
            researcher.QueueTurnsLeft = 0;
        }
    }
}
