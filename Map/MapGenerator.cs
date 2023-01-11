using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace AoeBoardgame
{
    /// <summary>
    /// For now we are assuming a 25x21 map
    /// </summary>
    class MapGenerator
    {
        private readonly TextureLibrary _textureLibrary;
        private readonly int _tileDimensions;

        private Map _map;

        public MapGenerator(TextureLibrary textureLibrary, int tileRelativeSize)
        {
            _textureLibrary = textureLibrary;
            _tileDimensions = tileRelativeSize * 4;
        }

        public Map GenerateRandom(int width, int height)
        {
            _map = new Map(width, height);

            MakeBaseTiles();

            AddRandomlyGeneratedTiles(TileType.Forest, 18, new Rectangle(0, 0, 13, 9)); // Top left
            AddRandomlyGeneratedTiles(TileType.Forest, 18, new Rectangle(13, 0, 13, 9)); // Top right

            AddRandomlyGeneratedTiles(TileType.Forest, 18, new Rectangle(0, 12, 13, 9)); // Bottom left
            AddRandomlyGeneratedTiles(TileType.Forest, 18, new Rectangle(13, 12, 13, 9)); // Bottom right

            AddRandomlyGeneratedTiles(TileType.Forest, 4, new Rectangle(0, 10, 4, 3)); // Left middle
            AddRandomlyGeneratedTiles(TileType.Forest, 4, new Rectangle(22, 10, 4, 3)); // Right middle

            AddRandomlyGeneratedTiles(TileType.Forest, 5, new Rectangle(8, 9, 10, 3)); // Middle

            AddRandomlyGeneratedTiles(TileType.GoldMine, 2, new Rectangle(0, 3, 10, 5)); // Top left
            AddRandomlyGeneratedTiles(TileType.GoldMine, 2, new Rectangle(15, 3, 10, 5)); // Top right

            AddRandomlyGeneratedTiles(TileType.GoldMine, 2, new Rectangle(0, 13, 10, 5)); // Bottom left
            AddRandomlyGeneratedTiles(TileType.GoldMine, 2, new Rectangle(15, 13, 10, 5)); // Bottom right

            AddRandomlyGeneratedTiles(TileType.GoldMine, 3, new Rectangle(11, 0, 3, 21)); // Middle

            AddRandomlyGeneratedTiles(TileType.IronMine, 2, new Rectangle(0, 3, 10, 5)); // Top left
            AddRandomlyGeneratedTiles(TileType.IronMine, 2, new Rectangle(15, 3, 10, 5)); // Top right

            AddRandomlyGeneratedTiles(TileType.IronMine, 2, new Rectangle(0, 13, 10, 5)); // Bottom left
            AddRandomlyGeneratedTiles(TileType.IronMine, 2, new Rectangle(15, 13, 10, 5)); // Bottom right

            AddRandomlyGeneratedTiles(TileType.IronMine, 3, new Rectangle(11, 0, 3, 21)); // Middle

            AddRandomlyGeneratedTiles(TileType.StoneMine, 1, new Rectangle(0, 3, 10, 5)); // Top left
            AddRandomlyGeneratedTiles(TileType.StoneMine, 1, new Rectangle(15, 3, 10, 5)); // Top right

            AddRandomlyGeneratedTiles(TileType.StoneMine, 1, new Rectangle(0, 13, 10, 5)); // Bottom left
            AddRandomlyGeneratedTiles(TileType.StoneMine, 1, new Rectangle(15, 13, 10, 5)); // Bottom right

            AddRandomlyGeneratedGaiaObjects<Deer>(3, new Rectangle(0, 1, 10, 7)); // Top left
            AddRandomlyGeneratedGaiaObjects<Deer>(3, new Rectangle(15, 1, 10, 7)); // Top right

            AddRandomlyGeneratedGaiaObjects<Deer>(3, new Rectangle(0, 13, 10, 7)); // Bottom left
            AddRandomlyGeneratedGaiaObjects<Deer>(3, new Rectangle(15, 13, 10, 7)); // Bottom right

            AddRandomlyGeneratedGaiaObjects<Deer>(2, new Rectangle(11, 0, 3, 21)); // Middle

            AddRandomlyGeneratedGaiaObjects<Boar>(1, new Rectangle(0, 1, 10, 7)); // Top left
            AddRandomlyGeneratedGaiaObjects<Boar>(1, new Rectangle(15, 1, 10, 7)); // Top right

            AddRandomlyGeneratedGaiaObjects<Boar>(1, new Rectangle(0, 13, 10, 7)); // Bottom left
            AddRandomlyGeneratedGaiaObjects<Boar>(1, new Rectangle(15, 13, 10, 7)); // Bottom right

            AddRandomlyGeneratedGaiaObjects<Boar>(1, new Rectangle(11, 0, 3, 21)); // Middle

            _map.SetSeed();

            return _map;
        }

        public Map GenerateFromSeed(string seed)
        {
            _map = new Map(25, 21)
            {
                Seed = seed
            };

            MakeBaseTiles();

            int currentTileId = 0;

            foreach (char c in seed.Split('_')[1])
            {
                if (int.TryParse(c.ToString(), out int emptyTiles))
                {
                    currentTileId += emptyTiles;
                    continue;
                }

                Tile currentTile = _map.Tiles.Single(e => e.Id == currentTileId);

                if (c == 'd')
                {
                    currentTile.SetObject(new Deer(_textureLibrary));
                }
                else if (c == 'b')
                {
                    currentTile.SetObject(new Boar(_textureLibrary));
                }
                else if (c == 'f')
                {
                    currentTile.SetType(TileType.Forest);
                }
                else if (c == 'g')
                {
                    currentTile.SetType(TileType.GoldMine);
                }
                else if (c == 'i')
                {
                    currentTile.SetType(TileType.IronMine);
                }
                else if (c == 's')
                {
                    currentTile.SetType(TileType.StoneMine);
                }

                currentTileId++;
            }

            return _map;
        }

        public void AddRandomlyGeneratedTiles(TileType tileType, int amount, Rectangle mapLocation)
        {
            for (var i = 0; i < amount; i++)
            {
                Tile tile = _map.GetRandomUnoccupiedTileInRange(mapLocation);
                tile.SetType(tileType);
            }
        }

        public void AddRandomlyGeneratedGaiaObjects<T>(int amount, Rectangle mapLocation) where T : GaiaObject
        {
            for (var i = 0; i < amount; i++)
            {
                Tile tile = _map.GetRandomUnoccupiedTileInRange(mapLocation);
                GaiaObject obj = (T)Activator.CreateInstance(typeof(T), _textureLibrary);
                tile.SetObject(obj);
            }
        }

        private void MakeBaseTiles()
        {
            int oddRowTileOffset = _tileDimensions / 2 + 1;
            int verticalTileOffset = _tileDimensions / 2 + _tileDimensions / 4 + 2;

            for (var y = 0; y < _map.Height; y++)
            {
                for (var x = 0; x < _map.Width; x++)
                {
                    bool evenRow = y % 2 == 0;
                    var tileLocation = new Point(_tileDimensions * x + 2 * x + (evenRow ? 0 : oddRowTileOffset), y * verticalTileOffset);
                    var tileSize = new Point(_tileDimensions, _tileDimensions);

                    var tile = new Tile(_textureLibrary, new Rectangle(tileLocation, tileSize));
                    tile.Id = _map.Tiles.Count;
                    tile.X = x;
                    tile.Y = y;
                    tile.SetType(TileType.Dirt);

                    _map.Tiles.Add(tile);
                }
            }
        }
    }
}
