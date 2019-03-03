using System;

namespace AoeBoardgame
{
    class Research
    {
        public ResearchEnum ResearchEnum { get; set; }
        public Action<Player> Effect { get; set; }
    }
}
