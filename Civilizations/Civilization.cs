using System.Collections.Generic;

namespace AoeBoardgame
{
    abstract class Civilization
    {
        protected readonly TextureLibrary TextureLibrary;

        protected Civilization(TextureLibrary textureLibrary)
        {
            TextureLibrary = textureLibrary;
        }

        public virtual IEnumerable<PlaceableObjectFactory> GetFactories(Player player)
        {
            var factories = new List<PlaceableObjectFactory>();

            factories.AddRange(new List<PlaceableObjectFactory>
            {
                // Buildings
                new TownCenterFactory(TextureLibrary),
                new TowerFactory(TextureLibrary),
                new LumberCampFactory(TextureLibrary),
                new BarracksFactory(TextureLibrary),
                new MineFactory(TextureLibrary),

                // Units
                new VillagerFactory(TextureLibrary)
            });

            return factories;
        }

        public virtual IEnumerable<ResourceGatherRate> GetBaseGatherRates()
        {
            return new List<ResourceGatherRate>
            {
                new ResourceGatherRate(Resource.Food, 5),
                new ResourceGatherRate(Resource.Wood, 5),
                new ResourceGatherRate(Resource.Gold, 5),
                new ResourceGatherRate(Resource.Iron, 5),
                new ResourceGatherRate(Resource.Stone, 5)
            };
        }

        public virtual IEnumerable<ResourceCollection> GetStartingResources()
        {
            return new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Food, 100),
                new ResourceCollection(Resource.Wood, 100),
                new ResourceCollection(Resource.Gold, 100),
                new ResourceCollection(Resource.Iron, 100),
                new ResourceCollection(Resource.Stone, 100)
            };
        }
    }
}
