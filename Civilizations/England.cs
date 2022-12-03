using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    /// <summary>
    /// Economic bonus: Faster wood gathering
    /// Military bonus: Archers +1 range
    /// Unique unit: Longbowman (long range archer)
    /// Unique tech: Agriculture
    /// </summary>
    class England : Civilization
    {
        public England(TextureLibrary textureLibrary, ResearchLibrary researchLibrary) : base(textureLibrary, researchLibrary) { }

        public override IEnumerable<PlaceableObjectFactory> GetFactories(Player player)
        {
            var factories = base.GetFactories(player).ToList();

            factories.Add(new LongbowmanFactory(TextureLibrary));

            return factories;
        }

        public override CastleFactory GetCastleFactory()
        {
            return new CastleFactory(TextureLibrary)
            {
                UnitTypesAllowedToMake = new List<Type>
                {
                    typeof(Longbowman)
                }
            };
        }

        public override void UnlockImperialAge(Player player)
        {
            base.UnlockImperialAge(player);

            player.AddAllowedResearch<Castle>(new List<ResearchEnum>
            {
                ResearchEnum.EliteLongbowmen,
                ResearchEnum.Agriculture
            });
        }

        public override ArcherFactory GetArcherFactory()
        {
            var factory = base.GetArcherFactory();

            factory.Range += 1;

            return factory;
        }

        public override IEnumerable<ResourceGatherRate> GetBaseGatherRates()
        {
            var gatherRates = base.GetBaseGatherRates();

            gatherRates.Single(e => e.Resource == Resource.Wood).GatherRate += 1;

            return gatherRates;
        }
    }
}
