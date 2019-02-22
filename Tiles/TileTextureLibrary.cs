using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class TextureLibrary
    {
        private readonly List<TileTexture> _tileTextures;
        private readonly List<TileObjectTexture> _objectTextures;
        private readonly List<TileColorTexture> _colorTextures;
        
        public TextureLibrary(ContentManager contentManager)
        {
            _tileTextures = new List<TileTexture>();
            _objectTextures = new List<TileObjectTexture>();
            _colorTextures = new List<TileColorTexture>();

            AddTileTextures(contentManager);
            AddObjectTextures(contentManager);
            AddColorTextures(contentManager);
        }

        public Texture2D GetTileObjectTextureByType(TileObjectType tileObjectType)
        {
            return _objectTextures.Find(e => e.TileObjectType == tileObjectType).Texture;
        }

        public Texture2D GetTileTextureByType(TileType tileType)
        {
            return _tileTextures.Find(e => e.TileType == tileType).Texture;
        }

        public Texture2D GetTileColorByType(TileColor tileColor)
        {
            return _colorTextures.Find(e => e.TileColor == tileColor).Texture;
        }

        private void AddTileTextures(ContentManager contentManager)
        {
            // Terrain
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.Dirt,
                Texture = contentManager.Load<Texture2D>("Tiles/Terrain/Dirt/dirt_06")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.Forest,
                Texture = contentManager.Load<Texture2D>("Tiles/Terrain/Grass/grass_12")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.StoneMine,
                Texture = contentManager.Load<Texture2D>("Tiles/Terrain/Dirt/dirt_18")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.GoldMine,
                Texture = contentManager.Load<Texture2D>("Tiles/Terrain/Dirt/dirt_20")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.IronMine,
                Texture = contentManager.Load<Texture2D>("Tiles/Terrain/Dirt/dirt_21")
            });

            // Buildings
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.ArcheryRange,
                Texture = contentManager.Load<Texture2D>("Tiles/Medieval/medieval_archery")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.Blacksmith,
                Texture = contentManager.Load<Texture2D>("Tiles/Medieval/medieval_blacksmith")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.Church,
                Texture = contentManager.Load<Texture2D>("Tiles/Medieval/medieval_church")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.LumberCamp,
                Texture = contentManager.Load<Texture2D>("Tiles/Medieval/medieval_lumber")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.Castle,
                Texture = contentManager.Load<Texture2D>("Tiles/Medieval/medieval_largeCastle")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.Farms,
                Texture = contentManager.Load<Texture2D>("Tiles/Medieval/medieval_windmill")
            });
        }

        private void AddObjectTextures(ContentManager contentManager)
        {
            // Resources
            _objectTextures.Add(new TileObjectTexture
            {
                TileObjectType = TileObjectType.Berries,
                Texture = contentManager.Load<Texture2D>("Objects/berries")
            });

            // Buildings
            _objectTextures.Add(new TileObjectTexture
            {
                TileObjectType = TileObjectType.TownCenter,
                Texture = contentManager.Load<Texture2D>("Objects/castle_small")
            });
            _objectTextures.Add(new TileObjectTexture
            {
                TileObjectType = TileObjectType.Tower,
                Texture = contentManager.Load<Texture2D>("Objects/watertower")
            });
            _objectTextures.Add(new TileObjectTexture
            {
                TileObjectType = TileObjectType.GuardTower,
                Texture = contentManager.Load<Texture2D>("Objects/tower")
            });
            _objectTextures.Add(new TileObjectTexture
            {
                TileObjectType = TileObjectType.Mine,
                Texture = contentManager.Load<Texture2D>("Objects/mine")
            });
            _objectTextures.Add(new TileObjectTexture
            {
                TileObjectType = TileObjectType.University,
                Texture = contentManager.Load<Texture2D>("Objects/oldBuilding")
            });
            _objectTextures.Add(new TileObjectTexture
            {
                TileObjectType = TileObjectType.Barracks,
                Texture = contentManager.Load<Texture2D>("Objects/mill_redBrick")
            });
        }

        private void AddColorTextures(ContentManager contentManager)
        {
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Blue,
                Texture = contentManager.Load<Texture2D>("Colors/blue")
            });
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Red,
                Texture = contentManager.Load<Texture2D>("Colors/red")
            });
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Green,
                Texture = contentManager.Load<Texture2D>("Colors/green")
            });
        }
    }
}
