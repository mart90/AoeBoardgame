using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class Player
    {
        public string Name { get; set; }
        public TileColor Color { get; set; }
        public bool IsActive { get; set; }
        public Civilization Civilization { get; set; }
        public bool IsLocalPlayer { get; set; }

        public IEnumerable<ResourceCollection> ResourceCollection { get; }
        public IEnumerable<ResourceCollection> ResourcesGatheredLastTurn { get; private set; }
        public IEnumerable<ResourceGatherRate> GatherRates { get; }
        public List<PlayerObject> OwnedObjects { get; }
        public int Age { get; set; }

        private IEnumerable<PlaceableObjectFactory> _factories;

        private ResearchLibrary _researchLibrary;

        public bool IsPopulationRevolting => ResourceCollection.Single(e => e.Resource == Resource.Gold).Amount < 0;

        public Player(string name, Civilization civilization, TileColor color, ResearchLibrary researchLibrary)
        {
            Name = name;
            Age = 1;
            Color = color;
            Civilization = civilization;
            _researchLibrary = researchLibrary;
            IsLocalPlayer = true;

            OwnedObjects = new List<PlayerObject>();

            ResourceCollection = Civilization.GetStartingResources();
            GatherRates = Civilization.GetBaseGatherRates();
            _factories = Civilization.GetFactories(this);
            ResourcesGatheredLastTurnReset();
        }

        public void MakeBuilding<T>(ICanMakeBuildings builder, Tile destinationTile) where T : PlayerObject
        {
            if (!builder.BuildingTypesAllowedToMake.Contains(typeof(T)))
            {
                return;
            }

            var newBuilding = AddAndGetPlaceableObject<T>();

            if (destinationTile.SeemsAccessible)
            {
                destinationTile.SetObject(newBuilding);
            }
        }

        public T AddAndGetPlaceableObject<T>() where T : PlayerObject
        {
            var factory = _factories.SingleOrDefault(e => e.Type == typeof(T));
            var newObj = (T)factory.Get(this);

            OwnedObjects.Add(newObj);
            return newObj;
        }

        public PlayerObject AddAndGetPlaceableObject(Type objectType)
        {
            var factory = _factories.SingleOrDefault(e => e.Type == objectType);
            var newObj = (PlayerObject)factory.Get(this);

            OwnedObjects.Add(newObj);
            return newObj;
        }

        public void ResourcesGatheredLastTurnReset()
        {
            ResourcesGatheredLastTurn = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Food, 0),
                new ResourceCollection(Resource.Wood, 0),
                new ResourceCollection(Resource.Gold, 0),
                new ResourceCollection(Resource.Iron, 0),
                new ResourceCollection(Resource.Stone, 0),
            };
        }

        public PlaceableObjectFactory GetFactoryByObjectType(Type type)
        {
            return _factories.Where(e => e.Type == type).Single();
        }

        public bool CanAfford(IEnumerable<ResourceCollection> cost)
        {
            foreach (ResourceCollection resourceCost in cost)
            {
                if (ResourceCollection.Single(e => e.Resource == resourceCost.Resource).Amount < resourceCost.Amount)
                {
                    return false;
                }
            }

            return true;
        }

        public void PayCost(IEnumerable<ResourceCollection> cost)
        {
            foreach (ResourceCollection resourceCost in cost)
            {
                ResourceCollection.Single(e => e.Resource == resourceCost.Resource).Amount -= resourceCost.Amount;
            }
        }
    }
}
