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
                        foreach (var playerObject in player.OwnedObjects)
                        {
                            playerObject.UpgradeToFeudalAge();
                        }

                        foreach (var factory in player.Factories)
                        {
                            factory.UpgradeToFeudalAge();
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.CastleAge,
                    Effect = player =>
                    {
                        foreach (var playerObject in player.OwnedObjects)
                        {
                            playerObject.UpgradeToCastleAge();
                        }

                        foreach (var factory in player.Factories)
                        {
                            factory.UpgradeToCastleAge();
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.ImperialAge,
                    Effect = player =>
                    {
                        foreach (var playerObject in player.OwnedObjects)
                        {
                            playerObject.UpgradeToImperialAge();
                        }

                        foreach (var factory in player.Factories)
                        {
                            factory.UpgradeToImperialAge();
                        }
                    }
                }
            };
        }
    }
}
