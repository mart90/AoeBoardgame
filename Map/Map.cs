﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public Tile SelectedTile => Tiles.SingleOrDefault(e => e.IsSelected);

        public Tile HoveredTile => Tiles.SingleOrDefault(e => e.IsHovered);

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in Tiles)
            {
                tile.Draw(spriteBatch);
            }
        }

        public IEnumerable<Tile> GetTilesByType(TileType tileType)
        {
            return Tiles.Where(e => e.Type == tileType);
        }

        public Tile GetTileByLocation(Point point)
        {
            var tiles = Tiles.FindAll(e => e.LocationSquareIncludesPoint(point));

            if (!tiles.Any())
            {
                return null;
            }
            if (tiles.Count == 1)
            {
                return tiles.First();
            }

            // Matched 2 tiles. This means the point is in one of the tiles' "corners"
            // Find out which tile's center is closer to the point
            Point tile1Center = tiles.First().Center;
            Point tile2Center = tiles.Last().Center;

            double distanceToTile1Center =
                Math.Sqrt(Math.Pow(Math.Abs(tile1Center.X - point.X), 2) +
                          Math.Pow(Math.Abs(tile1Center.Y - point.Y), 2));
            double distanceToTile2Center =
                Math.Sqrt(Math.Pow(Math.Abs(tile2Center.X - point.X), 2) +
                          Math.Pow(Math.Abs(tile2Center.Y - point.Y), 2));

            return distanceToTile1Center <= distanceToTile2Center ? tiles.First() : tiles.Last();
        }

        public Tile GetRandomUnoccupiedTile()
        {
            var dirtTiles = Tiles.Where(e => e.Type == TileType.Dirt);
            var randomTileNumber = _random.Next(0, dirtTiles.Count() - 1);
            return Tiles[randomTileNumber];
        }

        public Tile GetTileByCoordinates(int x, int y) => Tiles[y * Width + x];

        public Tile GetTileByObject(PlaceableObject obj)
        {
            Tile tile = Tiles.SingleOrDefault(e => e.Object == obj);

            if (tile == null)
            {
                // Object is part of an army or gathering

                return null;
            }

            return tile;
        }

        public int GetXCoordinate(Tile tile)
        {
            var tileId = Tiles.IndexOf(tile);
            return tileId % Width;
        }

        public int GetYCoordinate(Tile tile)
        {
            var tileId = Tiles.IndexOf(tile);
            return tileId / Width;
        }

        public IEnumerable<Tile> FindPathFromSelectedToHovered(ICanMove mover)
        {
            return new PathFinder(this).GetOptimalPath(SelectedTile, HoveredTile, mover);
        }

        public IEnumerable<Tile> FindPath(Tile source, Tile destination, ICanMove mover)
        {
            return new PathFinder(this).GetOptimalPath(source, destination, mover);
        }

        public IEnumerable<Tile> FindTilesInRangeOfSelected(int range)
        {
            return new PathFinder(this).GetAllTilesInRange(SelectedTile, range);
        }

        public void MoveObject(Tile source, Tile destination)
        {
            destination.SetObject(source.Object);
            source.SetObject(null);

            if (source.IsSelected)
            {
                source.IsSelected = false;
                destination.IsSelected = true;
            }
        }

        public Tile ProgressOnPath(ICanMove mover, IEnumerable<Tile> path)
        {
            int steps = path.Count() >= mover.Speed ? mover.Speed : path.Count();
            mover.StepsTakenThisTurn += steps;

            Tile endTile = path.ToList()[-1 + steps];
            MoveObject(GetTileByObject((PlaceableObject)mover), endTile);

            return endTile;
        }

        public void ResetFogOfWar()
        {
            Tiles.ForEach(e => e.HasFogOfWar = true);
        }
    }
}
