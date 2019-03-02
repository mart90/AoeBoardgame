using System.Collections.Generic;

namespace AoeBoardgame
{
    class Britons : Civilization
    {
        public Britons(TextureLibrary textureLibrary) : base(textureLibrary) { }

        public override IEnumerable<PlaceableObjectFactory> GetFactories()
        {
            var factories = new List<PlaceableObjectFactory>();

            factories.AddRange(new List<PlaceableObjectFactory>
            {
                new TownCenterFactory(TextureLibrary),
                new VillagerFactory(TextureLibrary)
            });

            return factories;
        }
    }
}
