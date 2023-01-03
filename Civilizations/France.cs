using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    /// <summary>
    /// Economic bonus: Cheaper farms
    /// Military bonus: Cavalry can move after attacking
    /// Unique unit: Throwing axeman (armor piercing ranged unit)
    /// Unique tech: Chivalry
    /// </summary>
    class France : Civilization
    {
        public France(TextureLibrary textureLibrary, ResearchLibrary researchLibrary) : base(textureLibrary, researchLibrary) { }

        public override IEnumerable<PlaceableObjectFactory> GetFactories(Player player)
        {
            var factories = base.GetFactories(player).ToList();

            factories.Add(new ThrowingAxemanFactory(TextureLibrary));

            return factories;
        }

        public override CastleFactory GetCastleFactory()
        {
            return new CastleFactory(TextureLibrary)
            {
                UnitTypesAllowedToMake = new List<Type>
                {
                    typeof(ThrowingAxeman)
                }
            };
        }

        public override void UnlockImperialAge(Player player)
        {
            base.UnlockImperialAge(player);

            player.AddAllowedResearch<Castle>(new List<ResearchEnum>
            {
                ResearchEnum.EliteThrowingAxemen,
                ResearchEnum.Chivalry
            });
        }

        public override FarmFactory GetFarmFactory()
        {
            var factory = base.GetFarmFactory();

            factory.Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 50)
            };

            return factory;
        }
    }
}
