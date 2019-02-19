using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AoeBoardgame
{
    class MapGenerator
    {
        private readonly Random _random;
        private readonly TextureLibrary _textureLibrary;
        private readonly int _tileSquareRootPixels;

        private Map _map;

        public MapGenerator(TextureLibrary textureLibrary, int tileRelativeSize)
        {
            _random = new Random();
            _textureLibrary = textureLibrary;
            _tileSquareRootPixels = tileRelativeSize * 4;
        }

        public Map GenerateMap(int width, int height)
        {
            _map = new Map();

            MakeBaseTiles(width, height);

            AddRandomlyGeneratedTiles(TileType.Forest, 0.25);
            AddRandomlyGeneratedTiles(TileType.IronMine, 0.05);
            AddRandomlyGeneratedTiles(TileType.StoneMine, 0.03);
            AddRandomlyGeneratedTiles(TileType.GoldMine, 0.03);

            return _map;
        }

        public void AddRandomlyGeneratedTiles(TileType tileType, double fractionOfMap)
        {
            int amountToAdd = (int) Math.Round(fractionOfMap * _map.TileCount);
            for (var i = 0; i < amountToAdd; i++)
            {
                var randomTileNumber = _random.Next(0, _map.TileCount - 1);
                _map.GetTileById(randomTileNumber).SetType(tileType);
            }
        }

        private void MakeBaseTiles(int width, int height)
        {
            _map.TileRows = new List<List<Tile>>();

            int oddRowTileOffset = _tileSquareRootPixels / 2 + 1;
            int verticalTileOffset = _tileSquareRootPixels / 2 + _tileSquareRootPixels / 4 + 2;

            for (var y = 0; y < height; y++)
            {
                var row = new List<Tile>();
                for (var x = 0; x < width; x++)
                {
                    bool evenRow = y % 2 == 0;
                    var tileLocation = new Point(_tileSquareRootPixels * x + 2 * x + (evenRow ? 0 : oddRowTileOffset), y * verticalTileOffset);
                    var tileSize = new Point(_tileSquareRootPixels, _tileSquareRootPixels);

                    var tile = new Tile(new Rectangle(tileLocation, tileSize), _textureLibrary);
                    tile.SetType(TileType.Dirt);
                    row.Add(tile);
                }
                _map.TileRows.Add(row);
            }
        }
    }
}
