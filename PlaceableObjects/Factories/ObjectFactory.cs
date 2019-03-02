using System;

namespace AoeBoardgame
{
    abstract class ObjectFactory
    {
        public abstract Type Type { get; }

        protected TextureLibrary TextureLibrary;
        protected Player Player;

        protected ObjectFactory(TextureLibrary textureLibrary, Player player)
        {
            TextureLibrary = textureLibrary;
            Player = player;

            SetDefaults();
        }

        protected abstract void SetDefaults();
    }
}
