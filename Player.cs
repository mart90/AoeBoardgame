using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class Player
    {
        public TileColor Color { get; set; }
        public bool IsActive { get; set; }
        public Civilization Civilization { get; set; }

        public ResourceCollection ResourceCollection { get; set; }
        public IEnumerable<Villager> Workers { get; private set; }
        public IEnumerable<Unit> Units { get; private set; }
        public IEnumerable<Building> Buildings { get; private set; }

        public IEnumerable<PlaceableObjectFactory> Factories { get; }

        public Player(Civilization civilization, TileColor color)
        {
            Color = color;
            Civilization = civilization;

            Workers = new List<Villager>();
            Units = new List<Unit>();
            Buildings = new List<Building>();

            ResourceCollection = new ResourceCollection();
            Factories = Civilization.GetFactories();
        }

        public T GetPlaceableObject<T>() where T : PlaceableObject
        {
            var factory = Factories.SingleOrDefault(e => e.Type == typeof(T));
            return (T)factory.Get(this);
        }
    }
}
