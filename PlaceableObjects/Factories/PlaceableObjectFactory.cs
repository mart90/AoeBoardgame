using System;

namespace AoeBoardgame
{
    abstract class PlaceableObjectFactory
    {
        public Type Type { get; protected set; }
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

        public abstract void UpgradeToFeudalAge();
        public abstract void UpgradeToCastleAge();
        public abstract void UpgradeToImperialAge();
    }
}
