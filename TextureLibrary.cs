using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class TextureLibrary
    {
        private readonly List<TileTexture> _tileTextures;
        private readonly List<PlaceableObjectTexture> _objectTextures;
        private readonly List<TileColorTexture> _colorTextures;
        
        public TextureLibrary(ContentManager contentManager)
        {
            _tileTextures = new List<TileTexture>();
            _objectTextures = new List<PlaceableObjectTexture>();
            _colorTextures = new List<TileColorTexture>();

            AddTileTextures(contentManager);
            AddObjectTextures(contentManager);
            AddColorTextures(contentManager);
        }

        public Texture2D GetObjectTextureByType(PlaceableObjectType tileObjectType)
        {
            return _objectTextures.Find(e => e.PlaceableObjectType == tileObjectType).Texture;
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
        }

        private void AddObjectTextures(ContentManager contentManager)
        {
            // Resources
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.Berries,
                Texture = contentManager.Load<Texture2D>("Objects/berries")
            });
            // TODO Boar

            // Buildings
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.TownCenter,
                Texture = contentManager.Load<Texture2D>("Objects/castle_small")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.Tower,
                Texture = contentManager.Load<Texture2D>("Objects/watertower")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.GuardTower,
                Texture = contentManager.Load<Texture2D>("Objects/tower")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.Mine,
                Texture = contentManager.Load<Texture2D>("Objects/mine")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.University,
                Texture = contentManager.Load<Texture2D>("Objects/oldBuilding")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.Barracks,
                Texture = contentManager.Load<Texture2D>("Objects/mill_redBrick")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.Stable,
                Texture = contentManager.Load<Texture2D>("Objects/house")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.Blacksmith,
                Texture = contentManager.Load<Texture2D>("Objects/oven")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.Church,
                Texture = contentManager.Load<Texture2D>("Objects/church")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.LumberCamp,
                Texture = contentManager.Load<Texture2D>("Objects/housing")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.Castle,
                Texture = contentManager.Load<Texture2D>("Objects/castle_large")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.Farms,
                Texture = contentManager.Load<Texture2D>("Objects/hedge")
            });

            // Units
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = PlaceableObjectType.Villager,
                Texture = contentManager.Load<Texture2D>("Objects/villager")
            });
        }

        private void AddColorTextures(ContentManager contentManager)
        {
            // Player colors
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

            // Misc
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Green, // Selected
                Texture = contentManager.Load<Texture2D>("Colors/green")
            });
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Teal, // Path
                Texture = contentManager.Load<Texture2D>("Colors/teal")
            });
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Pink, // Destination
                Texture = contentManager.Load<Texture2D>("Colors/pink")
            });
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Orange, // Max range
                Texture = contentManager.Load<Texture2D>("Colors/orange")
            });
        }
    }
}
