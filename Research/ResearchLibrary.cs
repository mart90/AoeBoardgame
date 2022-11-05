using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class ResearchLibrary
    {
        private readonly List<Research> _researchCollection;

        public Action<Player> GetEffectByResearchEnum(ResearchEnum researchEnum)
        {
            return _researchCollection.Single(e => e.ResearchEnum == researchEnum).Effect;
        }

        public ResearchLibrary()
        {
            _researchCollection = new List<Research>
            {
                new Research
                {
                    ResearchEnum = ResearchEnum.FeudalAge,
                    Effect = player => 
                    {
                        player.Age = 2;
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.CastleAge,
                    Effect = player =>
                    {
                        player.Age = 3;
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.ImperialAge,
                    Effect = player =>
                    {
                        player.Age = 4;
                    }
                }
            };
        }
    }
}
