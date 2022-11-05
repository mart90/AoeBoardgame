using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    abstract class PlaceableObjectFactory
    {
        public Type Type { get; protected set; }
        public IEnumerable<ResourceCollection> Cost { get; protected set; }
        public string UiName { get; set; }
        public string UiDescription { get; set; }
        public int TurnsToComplete { get; set; }

        protected TextureLibrary TextureLibrary;

        protected PlaceableObjectFactory(TextureLibrary textureLibrary)
        {
            TextureLibrary = textureLibrary;

            SetBaseValues();
        }

        protected abstract void SetBaseValues();

        public abstract PlaceableObject Get(Player player);

        public string TooltipString()
        {
            string str = "Cost: ";
            bool firstResource = true;

            foreach (ResourceCollection resourceCollection in Cost.Where(e => e.Amount > 0))
            {
                if (!firstResource)
                {
                    str += ", ";
                }
                str += $"{resourceCollection.Amount} {resourceCollection.Resource}";
                firstResource = false;
            }

            str += $"\nTurns to complete: {TurnsToComplete}";
            str += $"\n{UiDescription}";

            return str;
        }
    }
}
