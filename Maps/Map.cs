using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class Map
    {
        public List<Tile> Tiles { get; set; }
        public int Width { get; }
        public int Height { get; }

        private readonly Random _random;

        public Map(int width, int height)
        {
            _random = new Random();

            Width = width;
            Height = height;

            Tiles = new List<Tile>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in Tiles)
            {
                tile.Draw(spriteBatch);
            }
        }

        public List<Tile> GetTilesByType(TileType tileType)
        {
            return Tiles.FindAll(e => e.Type == tileType);
        }

        public Tile GetTileByLocation(Point point)
        {
            return Tiles.Find(e => e.IncludesPoint(point));
        }

        public Tile GetRandomUnoccupiedTile()
        {
            var dirtTiles = Tiles.FindAll(e => e.Type == TileType.Dirt);
            var randomTileNumber = _random.Next(0, dirtTiles.Count - 1);
            return Tiles[randomTileNumber];
        }
    }
}
