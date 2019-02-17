using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class Tile
    {
        public TileType Type { get; set; }
        public Rectangle Location { get; set; }
        public Texture2D Texture { get; private set; }

        private TextureLibrary _tileLibrary;

        public Tile(Rectangle location, TextureLibrary tileLibrary)
        {
            Location = location;
            _tileLibrary = tileLibrary;
        }

        public void SetGrass()
        {
            Type = TileType.Grass;
            Texture = _tileLibrary.Grass;
        }

        public void SetForest()
        {
            Type = TileType.Forest;
            Texture = _tileLibrary.Forest;
        }

        public void SetStoneMine()
        {
            Type = TileType.StoneMine;
            Texture = _tileLibrary.StoneMine;
        }
    }
}
