using System.Collections.Generic;

namespace AoeBoardgame
{
    interface ICanMakeResearch
    {
        IEnumerable<Research> AllowedResearch { get; set; }
    }
}
