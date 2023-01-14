using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui;

namespace AoeBoardgame
{
    class TextureLibrary
    {
        private readonly ImGUIRenderer _imGUIRenderer;

        private readonly List<TileTexture> _tileTextures;
        private readonly List<PlaceableObjectTexture> _objectTextures;
        private readonly List<TileColorTexture> _colorTextures;

        public Texture2D FogOfWar { get; private set; }
        public Texture2D TileUnderConstruction { get; private set; }
        public Texture2D SomethingInQueue { get; private set; }
        public Texture2D ArmyStar { get; private set; }
        public Texture2D HpBar { get; private set; }

        public TextureLibrary(ContentManager contentManager, ImGUIRenderer imGUIRenderer)
        {
            _tileTextures = new List<TileTexture>();
            _objectTextures = new List<PlaceableObjectTexture>();
            _colorTextures = new List<TileColorTexture>();
            _imGUIRenderer = imGUIRenderer;

            AddTileTextures(contentManager);
            AddObjectTextures(contentManager);
            AddColorTextures(contentManager);

            FogOfWar = contentManager.Load<Texture2D>("Colors/fow");
            TileUnderConstruction = contentManager.Load<Texture2D>("Misc/hammer");
            SomethingInQueue = contentManager.Load<Texture2D>("Misc/hourglass");
            ArmyStar = contentManager.Load<Texture2D>("Misc/star");
            HpBar = contentManager.Load<Texture2D>("Misc/hp");
        }

        public Texture2D GetObjectTextureByType(Type tileObjectType)
        {
            return _objectTextures.SingleOrDefault(e => e.PlaceableObjectType == tileObjectType)?.Texture;
        }

        public Texture2D GetTileTextureByType(TileType tileType)
        {
            return _tileTextures.Single(e => e.TileType == tileType).Texture;
        }

        public Texture2D GetTileColorByType(TileColor tileColor)
        {
            return _colorTextures.Single(e => e.TileColor == tileColor).Texture;
        }

        public IntPtr TextureToIntPtr(Texture2D texture)
        {
            return _imGUIRenderer.BindTexture(texture);
        }

        private void AddTileTextures(ContentManager contentManager)
        {
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.Dirt,
                Texture = contentManager.Load<Texture2D>("Tiles/dirt_06")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.Forest,
                Texture = contentManager.Load<Texture2D>("Tiles/grass_12")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.StoneMine,
                Texture = contentManager.Load<Texture2D>("Tiles/dirt_18")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.GoldMine,
                Texture = contentManager.Load<Texture2D>("Tiles/dirt_20")
            });
            _tileTextures.Add(new TileTexture
            {
                TileType = TileType.IronMine,
                Texture = contentManager.Load<Texture2D>("Tiles/dirt_21")
            });
        }

        private void AddObjectTextures(ContentManager contentManager)
        {
            // Gaia
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Deer),
                Texture = contentManager.Load<Texture2D>("Objects/deer")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Boar),
                Texture = contentManager.Load<Texture2D>("Objects/boar")
            });

            // Buildings
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(TownCenter),
                Texture = contentManager.Load<Texture2D>("Objects/castle_small")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Tower),
                Texture = contentManager.Load<Texture2D>("Objects/watertower")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(GuardTower),
                Texture = contentManager.Load<Texture2D>("Objects/tower")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Mine),
                Texture = contentManager.Load<Texture2D>("Objects/mine")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(University),
                Texture = contentManager.Load<Texture2D>("Objects/oldBuilding")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Barracks),
                Texture = contentManager.Load<Texture2D>("Objects/castle_open")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Stable),
                Texture = contentManager.Load<Texture2D>("Objects/mill_redBrick")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Blacksmith),
                Texture = contentManager.Load<Texture2D>("Objects/oven")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(LumberCamp),
                Texture = contentManager.Load<Texture2D>("Objects/housing")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Castle),
                Texture = contentManager.Load<Texture2D>("Objects/castle_large")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Farm),
                Texture = contentManager.Load<Texture2D>("Objects/farm")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(SiegeWorkshop),
                Texture = contentManager.Load<Texture2D>("Objects/siege_workshop")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Wonder),
                Texture = contentManager.Load<Texture2D>("Objects/archway")
            });

            // Units
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Villager),
                Texture = contentManager.Load<Texture2D>("Objects/villager2")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Archer),
                Texture = contentManager.Load<Texture2D>("Objects/archer")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Catapult),
                Texture = contentManager.Load<Texture2D>("Objects/catapult")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Knight),
                Texture = contentManager.Load<Texture2D>("Objects/knight")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Longbowman),
                Texture = contentManager.Load<Texture2D>("Objects/longbowman")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Pikeman),
                Texture = contentManager.Load<Texture2D>("Objects/pikeman")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Scout),
                Texture = contentManager.Load<Texture2D>("Objects/scout")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Swordsman),
                Texture = contentManager.Load<Texture2D>("Objects/swordsman")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(ThrowingAxeman),
                Texture = contentManager.Load<Texture2D>("Objects/throwing_axeman")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Trebuchet),
                Texture = contentManager.Load<Texture2D>("Objects/trebuchet")
            });
        }

        private void AddColorTextures(ContentManager contentManager)
        {
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Blue, // Player 1
                Texture = contentManager.Load<Texture2D>("Colors/blue")
            });
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Red, // Player 2
                Texture = contentManager.Load<Texture2D>("Colors/red")
            });
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
                TileColor = TileColor.Pink, // Range
                Texture = contentManager.Load<Texture2D>("Colors/pink")
            });
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Orange, // Destination
                Texture = contentManager.Load<Texture2D>("Colors/orange")
            });
            _colorTextures.Add(new TileColorTexture
            {
                TileColor = TileColor.Purple,
                Texture = contentManager.Load<Texture2D>("Colors/purple")
            });
        }
    }
}
