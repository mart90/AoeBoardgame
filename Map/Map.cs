using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class Map
    {
        public string Seed { get; set; }
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

        public Tile ViewedTile => Tiles.SingleOrDefault(e => e.IsViewed);

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

        public Tile GetRandomUnoccupiedTileInRange(Rectangle range)
        {
            List<Tile> dirtTiles = Tiles
                .Where(e => e.Type == TileType.Dirt && e.Object == null)
                .Where(e => e.X >= range.X
                    && e.Y >= range.Y
                    && e.X < range.X + range.Width
                    && e.Y < range.Y + range.Height)
                .ToList();

            var randomTileNumber = _random.Next(0, dirtTiles.Count() - 1);
            return dirtTiles[randomTileNumber];
        }

        public Tile GetTileByCoordinates(int x, int y) => Tiles.Single(e => e.X == x && e.Y == y);

        public Tile FindTileContainingObject(PlaceableObject obj)
        {
            Tile objectTile = Tiles.SingleOrDefault(e => e.Object == obj);

            if (objectTile != null)
            {
                return objectTile;
            }

            foreach (Tile tile in Tiles)
            {
                if (obj is ICanFormGroup unit && tile.Object is IContainsUnits unitContainer)
                {
                    if (unitContainer.Units.Contains(unit))
                    {
                        return tile;
                    }
                }
            }

            return null;
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

        public IEnumerable<Tile> FindTilesInRangeOfTile(Tile tile, int range, bool hasMinimumRange)
        {
            return new PathFinder(this).GetAllTilesInRange(tile, range, hasMinimumRange);
        }

        public void MoveMover(ICanMove mover, Tile destination)
        {
            Tile originTile = FindTileContainingObject((PlaceableObject)mover);

            if (mover is ICanFormGroup grouper && originTile.Object is IContainsUnits unitContainer)
            {
                unitContainer.Units.Remove(grouper);
                destination.SetObject((PlaceableObject)mover);

                if (unitContainer.Units.Count == 1  && !(unitContainer is IEconomicBuilding))
                {
                    // Disband group
                    originTile.SetObject((PlaceableObject)unitContainer.Units[0]);
                    ((PlayerObject)unitContainer).Owner.OwnedObjects.Remove((PlayerObject)unitContainer);
                }
            }
            else
            {
                destination.SetObject(originTile.Object);
                originTile.SetObject(null);
            }

            if (mover is IHasRange ranger)
            {
                ranger.RangeableTiles = FindTilesInRangeOfTile(destination, ranger.Range, ranger.HasMinimumRange);
            }

            if (originTile.IsSelected)
            {
                originTile.IsSelected = false;
                destination.IsSelected = true;
            }
        }

        public void MoveObjectSimple(Tile originTile, Tile destinationTile)
        {
            destinationTile.SetObject(originTile.Object);
            originTile.SetObject(null);
        }

        public void ResetFogOfWar()
        {
            Tiles.ForEach(e => e.HasFogOfWar = true);
        }
    }
}
