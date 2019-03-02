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

        public abstract IEnumerable<PlaceableObjectFactory> GetFactories();
    }
}
