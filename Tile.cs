using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class Tile
    {
        public TileType Type { get; set; }
        public Rectangle Location { get; set; }
        public Texture2D Texture { get; private set; }

        private readonly TextureLibrary _textureLibrary;

        public Tile(Rectangle location, TextureLibrary textureLibrary)
        {
            Location = location;
            _textureLibrary = textureLibrary;
        }

        public void SetType(TileType tileType)
        {
            Type = tileType;
            Texture = _textureLibrary.GetTileTextureByType(tileType);
        }
    }
}
