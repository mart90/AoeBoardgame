using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public void EmptyQueues()
        {
            for (var i = 0; i < OwnedObjects.Count; i++)
            {
                var playerObject = OwnedObjects[i];

                if (playerObject is IHasObjectQueue objectWithQueue)
                {
                    if (objectWithQueue.QueuedObject == null)
                    {
                        continue;
                    }

                    Type objectType = objectWithQueue.QueuedObject.ObjectType;
                    MethodInfo genericMethod = GetType().GetMethod(nameof(AddAndGetPlaceableObject));
                    MethodInfo runtimeMethod = genericMethod.MakeGenericMethod(objectType);

                    var newObject = (PlayerObject) runtimeMethod.Invoke(this, new object[] { });
                    objectWithQueue.QueuedObject.DestinationTile.SetObject(newObject);
                    objectWithQueue.QueuedObject = null;
                }
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
