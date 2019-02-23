using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AoeBoardgame
{
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

        public Map GenerateMap(int width, int height)
        {
            _map = new Map(width, height);

            MakeBaseTiles();

            AddRandomlyGeneratedTiles(TileType.Forest, 0.15);
            AddRandomlyGeneratedTiles(TileType.IronMine, 0.03);
            AddRandomlyGeneratedTiles(TileType.StoneMine, 0.02);
            AddRandomlyGeneratedTiles(TileType.GoldMine, 0.02);

            AddRandomlyGeneratedGaiaObjects(PlaceableObjectType.Berries, 0.05);

            return _map;
        }

        public void AddRandomlyGeneratedTiles(TileType tileType, double fractionOfMap)
        {
            int amountToAdd = (int) Math.Round(fractionOfMap * _map.Tiles.Count);

            for (var i = 0; i < amountToAdd; i++)
            {
                _map.GetRandomUnoccupiedTile().SetType(tileType);
            }
        }

        public void AddRandomlyGeneratedGaiaObjects(PlaceableObjectType objectType, double fractionOfDirtTiles)
        {
            List<Tile> dirtTiles = _map.GetTilesByType(TileType.Dirt);
            int amountToAdd = (int)Math.Round(fractionOfDirtTiles * dirtTiles.Count);

            for (var i = 0; i < amountToAdd; i++)
            {
                var obj = new GaiaObject(_textureLibrary, objectType);
                _map.GetRandomUnoccupiedTile().SetObject(obj);
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
                    tile.SetType(TileType.Dirt);

                    _map.Tiles.Add(tile);
                }
            }
        }
    }
}
