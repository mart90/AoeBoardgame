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
            var tiles = Tiles.FindAll(e => e.LocationSquareIncludesPoint(point));
            switch (tiles.Count)
            {
                case 0:
                    return null;
                case 1:
                    return tiles[0];
                default:
                {
                    // Matched 2 tiles. This means the point is in one of the tiles' "corners"
                    // Find out which tile's center is closer to the point
                    Point tile1Center = tiles[0].Center();
                    Point tile2Center = tiles[1].Center();
                    double distanceToTile1Center =
                        Math.Sqrt(Math.Pow(Math.Abs(tile1Center.X - point.X), 2) +
                                  Math.Pow(Math.Abs(tile1Center.Y - point.Y), 2));
                    double distanceToTile2Center =
                        Math.Sqrt(Math.Pow(Math.Abs(tile2Center.X - point.X), 2) +
                                  Math.Pow(Math.Abs(tile2Center.Y - point.Y), 2));

                    return distanceToTile1Center <= distanceToTile2Center ? tiles[0] : tiles[1];
                }
            }
        }

        public Tile GetRandomUnoccupiedTile()
        {
            var dirtTiles = Tiles.FindAll(e => e.Type == TileType.Dirt);
            var randomTileNumber = _random.Next(0, dirtTiles.Count - 1);
            return Tiles[randomTileNumber];
        }
    }
}
