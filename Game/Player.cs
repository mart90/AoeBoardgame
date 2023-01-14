using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class Player
    {
        public string Name { get; set; }
        public TileColor Color { get; set; }
        public TileColor UsedUnitColor { get; set; }
        public bool IsActive { get; set; }
        public Civilization Civilization { get; set; }
        public bool IsLocalPlayer { get; set; }

        public IEnumerable<ResourceCollection> ResourceStockpile { get; }
        public IEnumerable<ResourceCollection> ResourcesGatheredLastTurn { get; private set; }
        public IEnumerable<ResourceGatherRate> GatherRates { get; }
        public List<PlayerObject> OwnedObjects { get; }
        public int Age { get; set; }
        public int? WonderTimer { get; set; }

        public List<Tile> VisibleTiles => OwnedObjects.SelectMany(e => e.VisibleTiles).Distinct().ToList();

        public IEnumerable<PlaceableObjectFactory> Factories { get; private set; }

        public bool IsPopulationRevolting => ResourceStockpile.Single(e => e.Resource == Resource.Gold).Amount < 0;

        public Player(string name, Civilization civilization, TileColor color, TileColor usedUnitColor)
        {
            Name = name;
            Age = 1;
            Color = color;
            UsedUnitColor = usedUnitColor;
            Civilization = civilization;

            OwnedObjects = new List<PlayerObject>();

            ResourceStockpile = Civilization.GetStartingResources();
            GatherRates = Civilization.GetBaseGatherRates();
            Factories = Civilization.GetFactories(this);
            ResetResourcesGatheredLastTurn();
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
            var factory = Factories.SingleOrDefault(e => e.Type == typeof(T));
            var newObj = (T)factory.Get(this);

            OwnedObjects.Add(newObj);
            return newObj;
        }

        public PlayerObject AddAndGetPlaceableObject(Type objectType)
        {
            var factory = Factories.SingleOrDefault(e => e.Type == objectType);
            var newObj = (PlayerObject)factory.Get(this);

            OwnedObjects.Add(newObj);
            return newObj;
        }

        public void ResetResourcesGatheredLastTurn()
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

        public void ResetAttackers()
        {
            foreach (IAttacker attacker in OwnedObjects.Where(e => e is IAttacker))
            {
                attacker.HasAttackedThisTurn = false;
            }
        }

        public PlaceableObjectFactory GetFactoryByObjectType(Type type)
        {
            return Factories.Where(e => e.Type == type).Single();
        }

        public bool CanAfford(IEnumerable<ResourceCollection> cost)
        {
            foreach (ResourceCollection resourceCost in cost)
            {
                if (ResourceStockpile.Single(e => e.Resource == resourceCost.Resource).Amount < resourceCost.Amount)
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryPayCost(IEnumerable<ResourceCollection> cost)
        {
            foreach (ResourceCollection resourceCost in cost)
            {
                int resourceBank = ResourceStockpile.Single(e => e.Resource == resourceCost.Resource).Amount;
                
                if (resourceBank < resourceCost.Amount)
                {
                    return false;
                }
            }

            PayCost(cost);
            return true;
        }

        public void PayCost(IEnumerable<ResourceCollection> cost)
        {
            foreach (ResourceCollection resourceCost in cost)
            {
                ResourceStockpile.Single(e => e.Resource == resourceCost.Resource).Amount -= resourceCost.Amount;
            }
        }

        public void AddAllowedUnits<T>(List<Type> unitTypes)
            where T : PlayerObject, ICanMakeUnits
        {
            ICanMakeUnitsFactory factory = (ICanMakeUnitsFactory)GetFactoryByObjectType(typeof(T));

            foreach (Type type in unitTypes)
            {
                factory.UnitTypesAllowedToMake.Add(type);
            }

            foreach (T trainer in OwnedObjects.Where(e => e is T))
            {
                foreach (Type type in unitTypes)
                {
                    trainer.UnitTypesAllowedToMake.Add(type);
                }
            }
        }

        public void AddAllowedBuildings<T>(List<Type> buildingTypes)
            where T : PlayerObject, ICanMakeBuildings
        {
            ICanMakeBuildingsFactory factory = (ICanMakeBuildingsFactory)GetFactoryByObjectType(typeof(T));

            foreach (Type type in buildingTypes)
            {
                factory.BuildingTypesAllowedToMake.Add(type);
            }

            foreach (T builder in OwnedObjects.Where(e => e is T))
            {
                foreach (Type type in buildingTypes)
                {
                    builder.BuildingTypesAllowedToMake.Add(type);
                }
            }
        }

        public void AddAllowedResearch<T>(List<ResearchEnum> researchTypes)
            where T : PlayerObject, ICanMakeResearch
        {
            ICanMakeResearchFactory factory = (ICanMakeResearchFactory)GetFactoryByObjectType(typeof(T));

            foreach (ResearchEnum type in researchTypes)
            {
                factory.ResearchAllowedToMake.Add(type);

                foreach (T researcher in OwnedObjects.Where(e => e is T))
                {
                    researcher.ResearchAllowedToMake.Add(type);
                }
            }
        }
    }
}
