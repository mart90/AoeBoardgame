﻿using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class Player
    {
        public TileColor Color { get; set; }
        public bool IsActive { get; set; }
        public Civilization Civilization { get; set; }

        public ResourceCollection ResourceCollection { get; set; }
        public List<PlayerObject> OwnedObjects { get; set; }

        public IEnumerable<PlaceableObjectFactory> Factories { get; }

        private ResearchLibrary _researchLibrary;

        public Player(Civilization civilization, TileColor color, ResearchLibrary researchLibrary)
        {
            Color = color;
            Civilization = civilization;
            _researchLibrary = researchLibrary;

            OwnedObjects = new List<PlayerObject>();

            ResourceCollection = new ResourceCollection();
            Factories = Civilization.GetFactories();
        }

        public void MakeBuilding<T>(ICanMakeBuildings builder, Tile destinationTile) where T : PlayerObject
        {
            if (!builder.BuildingTypesAllowedToMake.Contains(typeof(T)))
            {
                return;
            }

            var newBuilding = AddAndGetPlaceableObject<T>();

            if (destinationTile.IsAccessible)
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
    }
}
