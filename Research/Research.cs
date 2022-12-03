using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class Research
    {
        public ResearchEnum ResearchEnum { get; set; }
        public string UiName { get; set; }
        public string UiDescription { get; set; }
        public List<ResourceCollection> Cost { get; set; }
        public int TurnsToComplete { get; set; }
        public Action<Player> Effect { get; set; }

        public string TooltipString()
        {
            string str = "Cost: ";
            bool firstResource = true;

            foreach (ResourceCollection resourceCollection in Cost.Where(e => e.Amount > 0))
            {
                if (!firstResource)
                {
                    str += ", ";
                }
                str += $"{resourceCollection.Amount} {resourceCollection.Resource}";
                firstResource = false;
            }

            str += $"\nTurns to complete: {TurnsToComplete}";
            str += $"\n{UiDescription}";

            return str;
        }
    }
}
