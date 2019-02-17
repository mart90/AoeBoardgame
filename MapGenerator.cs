using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AoeBoardgame
{
    class MapGenerator
    {
        private TextureLibrary _textureLibrary;
        private int _tileSquareRootPixels;

        private Map _map;

        public MapGenerator(TextureLibrary textureLibrary, int tileRelativeSize)
        {
            _textureLibrary = textureLibrary;
            _tileSquareRootPixels = tileRelativeSize * 4;
        }

        public Map GenerateMap(int width, int height)
        {
            _map = new Map();

            MakeBaseTiles(width, height);

            AddForests();
            AddStoneMines();

            return _map;
        }

        private void AddForests()
        {
            int tileCount = _map.TileCount;
            int amountOfForests = (int)Math.Round((double)tileCount / 5);

            Random rdm = new Random();
            for (var i = 0; i < amountOfForests; i++)
            {
                _map.GetTileById(rdm.Next(0, tileCount - 1)).SetForest();
            }
        }

        private void AddStoneMines()
        {
            int tileCount = _map.TileCount;
            int amountOfStoneMines = (int)Math.Round((double)tileCount / 20);

            Random rdm = new Random();
            for (var i = 0; i < amountOfStoneMines; i++)
            {
                _map.GetTileById(rdm.Next(0, tileCount - 1)).SetStoneMine();
            }
        }

        private void MakeBaseTiles(int width, int height)
        {
            _map.TileRows = new List<List<Tile>>();

            int oddRowTileOffset = _tileSquareRootPixels / 2 + 1;
            int verticalTileOffset = _tileSquareRootPixels / 2 + _tileSquareRootPixels / 4 + 2;

            for (var y = 0; y < width; y++)
            {
                var row = new List<Tile>();
                for (var x = 0; x < height; x++)
                {
                    bool evenRow = y % 2 == 0;
                    var tileLocation = new Point(_tileSquareRootPixels * x + 2 * x + (evenRow ? 0 : oddRowTileOffset), y * verticalTileOffset);
                    var tileSize = new Point(_tileSquareRootPixels, _tileSquareRootPixels);

                    var tile = new Tile(new Rectangle(tileLocation, tileSize), _textureLibrary);
                    tile.SetGrass();
                    row.Add(tile);
                }
                _map.TileRows.Add(row);
            }
        }
    }
}
