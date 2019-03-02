using System;

namespace AoeBoardgame
{
    abstract class PlaceableObjectFactory
    {
        public abstract Type Type { get; }
        public abstract ResourceCollection Cost { get; protected set; }

        protected TextureLibrary TextureLibrary;
        protected Player Player;

        protected PlaceableObjectFactory(TextureLibrary textureLibrary)
        {
            TextureLibrary = textureLibrary;
            SetDefaults();
        }

        protected abstract void SetDefaults();

        public abstract PlaceableObject Get(Player player);

        public abstract void AdvanceToFeudalAge();
        public abstract void AdvanceToCastleAge();
        public abstract void AdvanceToImperialAge();
    }
}
