using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeResearch : IHasQueue
    {
        Research ResearchQueued { get; set; }
        IEnumerable<Research> AllowedResearch { get; set; }
    }
}
