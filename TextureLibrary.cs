using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class TextureLibrary
    {
        private readonly List<TileTexture> _tileTextures;
        
        public TextureLibrary(ContentManager contentManager)
        {
            _tileTextures = new List<TileTexture>
            {
                new TileTexture
                {
                    TileType = TileType.Dirt,
                    Texture = contentManager.Load<Texture2D>("Tiles/Terrain/Dirt/dirt_06")
                },
                new TileTexture
                {
                    TileType = TileType.Forest,
                    Texture = contentManager.Load<Texture2D>("Tiles/Terrain/Grass/grass_12")
                },
                new TileTexture
                {
                    TileType = TileType.StoneMine,
                    Texture = contentManager.Load<Texture2D>("Tiles/Terrain/Dirt/dirt_18")
                },
                new TileTexture
                {
                    TileType = TileType.GoldMine,
                    Texture = contentManager.Load<Texture2D>("Tiles/Terrain/Dirt/dirt_20")
                },
                new TileTexture
                {
                    TileType = TileType.IronMine,
                    Texture = contentManager.Load<Texture2D>("Tiles/Terrain/Dirt/dirt_21")
                },
            };
        }

        public Texture2D GetTileTextureByType(TileType tileType)
        {
            return _tileTextures.Find(e => e.TileType == tileType).Texture;
        }
    }
}
