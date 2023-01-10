using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AoeBoardgame
{
    partial class PathFinder
    {
        private readonly Map _map;
        private readonly Queue<Node> _nodeQueue;

        /// <summary>
        /// Lowest number of nodes needed to reach each tile. Updated during search. Used to filter out inefficient paths
        /// </summary>
        private readonly List<List<int>> _nodeScores;

        public PathFinder(Map map)
        {
            _map = map;
            _nodeQueue = new Queue<Node>();
            _nodeScores = new List<List<int>>();

            for (int y = 0; y < _map.Height; y++)
            {
                var nodeScoreRow = new List<int>();
                for (int x = 0; x < _map.Width; x++)
                {
                    nodeScoreRow.Add(-1); // -1 = Unvisited
                }
                _nodeScores.Add(nodeScoreRow);
            }
        }

        public IEnumerable<Tile> GetAllTilesInRange(Tile originTile, int range, bool hasMinimumRange = false)
        {
            List<Tile> tiles = GetAllTilesInRangeIgnoreObstacles(originTile, range).ToList();

            var rangeableTiles = new List<Tile>();

            foreach (Tile destinationTile in tiles)
            {
                rangeableTiles.AddRange(GetRangeableTilesStraightLine(originTile, destinationTile));
            }

            if (hasMinimumRange)
            {
                rangeableTiles.RemoveAll(e => GetAdjacentTiles(originTile).Contains(e));
            }

            return rangeableTiles.Distinct();
        }

        /// <summary>
        /// Breadth-first search
        /// </summary>
        public IEnumerable<Tile> GetOptimalPath(Tile originTile, Tile destinationTile, ICanMove mover)
        {
            Node originNode = GetNodeByTileId(originTile.Id);
            Node destinationNode = GetNodeByTileId(destinationTile.Id);

            _nodeScores[originNode.Y][originNode.X] = 0;
            _nodeQueue.Enqueue(originNode);

            while (_nodeQueue.Count > 0)
            {
                Node currentNode = _nodeQueue.Dequeue();
                IEnumerable<Node> children = GetAdjacentNodesMover(currentNode, mover);

                foreach (var child in children)
                {
                    if (child.X == destinationNode.X && child.Y == destinationNode.Y)
                    {
                        var bestPath = child.NodesVisited;
                        bestPath.RemoveAt(0);
                        bestPath.Add(destinationNode);
                        return ConvertNodesToTiles(bestPath);
                    }

                    int childScore = _nodeScores[child.Y][child.X];
                    if (childScore != -1 && childScore <= child.NodesVisited.Count) 
                    {
                        // There is another path that reached this tile in less (or an equal amount of) steps. Discard
                        continue;
                    }

                    _nodeScores[child.Y][child.X] = child.NodesVisited.Count;
                    
                    if (!child.PathStopsHere)
                    {
                        _nodeQueue.Enqueue(child);
                    }
                }
            }

            // No path found
            return null; 
        }

        public IEnumerable<Tile> GetAdjacentTiles(Tile originTile)
        {
            Node originNode = GetNodeByTileId(originTile.Id);
            IEnumerable<Node> children = GetAdjacentNodes(originNode);
            return ConvertNodesToTiles(children);
        }

        /// <summary>
        /// Retreats in the direction opposite to where the attack came from if that tile is free. If it's not free, we check for free tiles going clockwise
        /// </summary>
        public Tile GetRetreatingTile(Tile originTile, Tile attackerTile)
        {
            Direction retreatDirection;

            Node originNode = GetNodeByTileId(originTile.Id);
            Node attackerNode = GetNodeByTileId(attackerTile.Id);

            int originTileX = originNode.X;
            int originTileY = originNode.Y;

            int attackerTileX = attackerNode.X;
            int attackerTileY = attackerNode.Y;

            bool evenRow = originTileY % 2 == 0;

            if (originTileX < attackerTileX && originTileY == attackerTileY)
            {
                retreatDirection = Direction.West;
            }
            else if (originTileX < attackerTileX && originTileY > attackerTileY)
            {
                retreatDirection = Direction.SouthWest;
            }
            else if (originTileX < attackerTileX && originTileY < attackerTileY)
            {
                retreatDirection = Direction.NorthWest;
            }
            else if (originTileX > attackerTileX && originTileY == attackerTileY)
            {
                retreatDirection = Direction.East;
            }
            else if (originTileX > attackerTileX && originTileY > attackerTileY)
            {
                retreatDirection = Direction.SouthEast;
            }
            else if (originTileX > attackerTileX && originTileY < attackerTileY)
            {
                retreatDirection = Direction.NorthEast;
            }
            else if (originTileX == attackerTileX && originTileY < attackerTileY && evenRow)
            {
                retreatDirection = Direction.NorthWest;
            }
            else if (originTileX == attackerTileX && originTileY < attackerTileY && !evenRow)
            {
                retreatDirection = Direction.NorthEast;
            }
            else if (originTileX == attackerTileX && originTileY > attackerTileY && evenRow)
            {
                retreatDirection = Direction.SouthWest;
            }
            else // originTileX == attackerTileX && originTileY > attackerTileY && !evenRow
            {
                retreatDirection = Direction.SouthEast;
            }

            Tile destinationTile;

            Node destinationNode = TakeStep(originNode, retreatDirection);

            if (destinationNode != null)
            {
                destinationTile = NodeToTile(destinationNode);

                if (destinationTile.IsEmpty)
                {
                    return destinationTile;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                retreatDirection = GetNextDirectionClockwise(retreatDirection);

                destinationNode = TakeStep(originNode, retreatDirection);
                
                if (destinationNode == null)
                {
                    continue;
                }

                destinationTile = NodeToTile(destinationNode);

                if (destinationTile.IsEmpty)
                {
                    return destinationTile;
                }
            }

            // No tiles to retreat to
            return null;
        }

        private IEnumerable<Tile> GetAllTilesInRangeIgnoreObstacles(Tile originTile, int range)
        {
            var nodes = new List<Node>();
            Node originNode = GetNodeByTileId(originTile.Id);

            _nodeScores[originNode.Y][originNode.X] = 0;
            _nodeQueue.Enqueue(originNode);

            while (_nodeQueue.Count > 0)
            {
                Node currentNode = _nodeQueue.Dequeue();

                if (currentNode.NodesVisited.Count == range)
                {
                    return ConvertNodesToTiles(nodes);
                }

                IEnumerable<Node> children = GetAdjacentNodes(currentNode);

                foreach (var child in children)
                {
                    int childScore = _nodeScores[child.Y][child.X];
                    if (childScore != -1 && childScore <= child.NodesVisited.Count)
                    {
                        // There is another path that reached this tile in less (or an equal amount of) steps. Discard
                        continue;
                    }

                    nodes.Add(child);

                    _nodeScores[child.Y][child.X] = child.NodesVisited.Count;
                    _nodeQueue.Enqueue(child);
                }
            }

            return null;
        }

        private IEnumerable<Tile> ConvertNodesToTiles(IEnumerable<Node> nodes)
        {
            return nodes.Select(e => _map.GetTileByCoordinates(e.X, e.Y)).ToList();
        }

        private IEnumerable<Node> GetAdjacentNodes(Node node)
        {
            var accessibleChildren = new List<Node>();
            List<Direction> directionsToGo = node.GetNewDirections();

            foreach (var direction in directionsToGo)
            {
                Node child = TakeStep(node, direction);
                if (child == null)
                {
                    continue;
                }

                accessibleChildren.Add(child);
            }

            return accessibleChildren;
        }

        private IEnumerable<Node> GetAdjacentNodesMover(Node node, ICanMove mover)
        {
            var accessibleChildren = new List<Node>();
            List<Direction> directionsToGo = node.GetNewDirections();

            foreach (var direction in directionsToGo)
            {
                Node child = TakeStep(node, direction);
                if (child == null)
                {
                    continue;
                }

                Tile childTile = NodeToTile(child);

                if (childTile.SeemsAccessible)
                {
                    accessibleChildren.Add(child);
                }
                else if (childTile.Object != null)
                {
                    child.PathStopsHere = true;

                    if (((PlayerObject)mover).CanAttack(childTile.Object))
                    {
                        accessibleChildren.Add(child);
                    }
                    else if (((PlayerObject)mover).CanMergeWith(childTile.Object))
                    {
                        accessibleChildren.Add(child);
                    }
                }
            }

            return accessibleChildren;
        }

        private Node TakeStep(Node parentNode, Direction direction)
        {
            bool evenRow = parentNode.Y % 2 == 0;
            int childNodeX = 0, childNodeY = 0;

            switch (direction)
            {
                case Direction.NorthEast:
                    childNodeY = parentNode.Y - 1;
                    childNodeX = evenRow ? parentNode.X : parentNode.X + 1;
                    break;
                case Direction.East:
                    childNodeY = parentNode.Y;
                    childNodeX = parentNode.X + 1;
                    break;
                case Direction.SouthEast:
                    childNodeY = parentNode.Y + 1;
                    childNodeX = evenRow ? parentNode.X : parentNode.X + 1;
                    break;
                case Direction.SouthWest:
                    childNodeY = parentNode.Y + 1;
                    childNodeX = evenRow ? parentNode.X - 1 : parentNode.X;
                    break;
                case Direction.West:
                    childNodeY = parentNode.Y;
                    childNodeX = parentNode.X - 1;
                    break;
                case Direction.NorthWest:
                    childNodeY = parentNode.Y - 1;
                    childNodeX = evenRow ? parentNode.X - 1 : parentNode.X;
                    break;
            }

            if (childNodeX >= _map.Width || childNodeY >= _map.Height || childNodeX < 0 || childNodeY < 0)
            {
                // We stepped off the map
                return null;
            }

            return new Node(childNodeX, childNodeY, parentNode, direction);
        }

        private Tile NodeToTile(Node node)
        {
            return _map.Tiles[node.Y * _map.Width + node.X];
        }

        private Node GetNodeByTileId(int id)
        {
            var x = id % _map.Width;
            var y = id / _map.Width;
            return new Node(x, y);
        }

        private Tile GetTileInDirection(Tile originTile, Direction direction)
        {
            Node originNode = new Node(originTile.X, originTile.Y);
            Node destinationNode = TakeStep(originNode, direction);

            if (destinationNode == null)
            {
                return null;
            }

            return NodeToTile(destinationNode);
        }

        // TODO refactor, optimize if needed
        private IEnumerable<Tile> GetRangeableTilesStraightLine(Tile originTile, Tile destinationTile)
        {
            var rangeableTiles = new List<Tile>();

            Point originCenter = originTile.Center;
            Point destinationCenter = destinationTile.Center;

            // Positive diffX means destination is RIGHT of origin
            int diffX = destinationCenter.X - originCenter.X;

            // Positive diffY means destination is BELOW origin
            int diffY = destinationCenter.Y - originCenter.Y;

            double yPerX;

            if (diffX != 0)
            {
                yPerX = (double)diffY / (double)diffX;
            }
            else
            {
                yPerX = (double)diffY < 0 ? -10 : 10;
            }

            (bool edgeCaseRoadIsClear, int yAdjustment) = CheckForEdgeCaseObstacles(originTile, destinationTile, diffX, diffY);

            if (!edgeCaseRoadIsClear)
            {
                return new List<Tile>();
            }

            int y;
            Point point;

            if (diffX > 0)
            {
                for (int x = 1; x < diffX; x++)
                {
                    y = yAdjustment + (int)Math.Round(x * yPerX) + originCenter.Y;

                    point = new Point(x + originCenter.X, y);

                    Tile tile = _map.GetTileByLocation(point);

                    if (tile == null || tile == originTile || rangeableTiles.Contains(tile))
                    {
                        continue;
                    }

                    rangeableTiles.Add(tile);

                    if (tile.Type != TileType.Dirt)
                    {
                        break;
                    }
                }
            }
            else if (diffX < 0)
            {
                for (int x = -1; x > diffX; x--)
                {
                    y = yAdjustment + (int)Math.Round(x * yPerX) + originCenter.Y;

                    point = new Point(x + originCenter.X, y);

                    Tile tile = _map.GetTileByLocation(point);

                    if (tile == null || tile == originTile || rangeableTiles.Contains(tile))
                    {
                        continue;
                    }

                    rangeableTiles.Add(tile);

                    if (tile.Type != TileType.Dirt)
                    {
                        break;
                    }
                }
            }
            else // diffX == 0, meaning the destination is directly north or south
            {
                if (diffY > 0)
                {
                    for (int i = 1; i * yPerX < diffY; i++)
                    {
                        y = (int)Math.Round(i * yPerX) + originCenter.Y;

                        point = new Point(originCenter.X, y);

                        Tile tile = _map.GetTileByLocation(point);

                        if (tile == null || tile == originTile || rangeableTiles.Contains(tile))
                        {
                            continue;
                        }

                        rangeableTiles.Add(tile);

                        if (tile.Type != TileType.Dirt)
                        {
                            break;
                        }
                    }
                }
                else if (diffY < 0)
                {
                    for (int i = 1; i * yPerX > diffY; i++)
                    {
                        y = (int)Math.Round(i * yPerX) + originCenter.Y;

                        point = new Point(originCenter.X, y);

                        Tile tile = _map.GetTileByLocation(point);

                        if (tile == null || tile == originTile || rangeableTiles.Contains(tile))
                        {
                            continue;
                        }

                        rangeableTiles.Add(tile);

                        if (tile.Type != TileType.Dirt)
                        {
                            break;
                        }
                    }
                }
            }

            return rangeableTiles;
        }

        private (bool roadIsClear, int yAdjustment) CheckForEdgeCaseObstacles(Tile originTile, Tile destinationTile, int diffX, int diffY)
        {
            // In the following 4 cases, the destination is directly below or above the origin
            // Rangers can shoot around obstacles on one side to reach the destination tile, but not in between them
            // The calling method would sneak in between them and not notice the obstacles
            // With Range 2, Abs(diffY) = 88: https://i.imgur.com/D9EX2f0.png, https://i.imgur.com/u9qI4Us.png
            if (diffY == 88 && diffX == 0)
            {
                Tile sw = GetTileInDirection(originTile, Direction.SouthWest);
                Tile se = GetTileInDirection(originTile, Direction.SouthEast);

                return sw?.Type == TileType.Dirt || se?.Type == TileType.Dirt ?
                    (true, 0) : (false, 0);
            }
            else if (diffY == -88 && diffX == 0)
            {
                Tile nw = GetTileInDirection(originTile, Direction.NorthWest);
                Tile ne = GetTileInDirection(originTile, Direction.NorthEast);

                return nw?.Type == TileType.Dirt || ne?.Type == TileType.Dirt ?
                    (true, 0) : (false, 0);
            }
            // With Range 4, Abs(diffY) = 176: https://i.imgur.com/AB3WCCK.png, https://i.imgur.com/gzvjCVC.png, https://i.imgur.com/FbVjf8e.png
            else if (diffY == 176 && diffX == 0)
            {
                Tile sw = GetTileInDirection(originTile, Direction.SouthWest);
                Tile se = GetTileInDirection(originTile, Direction.SouthEast);
                Tile swFar = GetTileInDirection(destinationTile, Direction.NorthWest);
                Tile seFar = GetTileInDirection(destinationTile, Direction.NorthEast);

                return (sw?.Type == TileType.Dirt && swFar?.Type == TileType.Dirt) || (se?.Type == TileType.Dirt && seFar?.Type == TileType.Dirt) ?
                    (true, 0) : (false, 0);
            }
            else if (diffY == -176 && diffX == 0)
            {
                Tile nw = GetTileInDirection(originTile, Direction.NorthWest);
                Tile ne = GetTileInDirection(originTile, Direction.NorthEast);
                Tile nwFar = GetTileInDirection(destinationTile, Direction.SouthWest);
                Tile neFar = GetTileInDirection(destinationTile, Direction.SouthEast);

                return (nw?.Type == TileType.Dirt && nwFar?.Type == TileType.Dirt) || (ne?.Type == TileType.Dirt && neFar?.Type == TileType.Dirt) ?
                    (true, 0) : (false, 0);
            }

            // In the following 8 cases, the destination is diagonally below or above the origin
            // The same rules apply in the game (can shoot around, not in between)
            // In this case, when going diagonally, the calling method DOES run into the obstacles (reason: pixels) and an adjustment of Y is needed to go around them
            // https://i.imgur.com/qf7a2ja.png, https://i.imgur.com/hkuk00A.png, https://i.imgur.com/PuIjfYc.png
            else if (diffY == 44 && diffX == 87) // 2 range, SouthEast
            {
                Tile e = GetTileInDirection(originTile, Direction.East);
                Tile se = GetTileInDirection(originTile, Direction.SouthEast);

                if (e?.Type != TileType.Dirt && se?.Type != TileType.Dirt)
                {
                    return (false, 0);
                }

                if (e?.Type != TileType.Dirt)
                {
                    return (true, 2);
                }
                else if (se?.Type != TileType.Dirt)
                {
                    return (true, -2);
                }
                else
                {
                    return (true, 0);
                }
            }
            else if (diffY == -44 && diffX == 87) // 2 range, NorthEast
            {
                Tile e = GetTileInDirection(originTile, Direction.East);
                Tile ne = GetTileInDirection(originTile, Direction.NorthEast);

                if (e?.Type != TileType.Dirt && ne?.Type != TileType.Dirt)
                {
                    return (false, 0);
                }

                if (e?.Type != TileType.Dirt)
                {
                    return (true, -2);
                }
                else if (ne?.Type != TileType.Dirt)
                {
                    return (true, 2);
                }
                else
                {
                    return (true, 0);
                }
            }
            else if (diffY == 44 && diffX == -87) // 2 range, SouthWest
            {
                Tile w = GetTileInDirection(originTile, Direction.West);
                Tile sw = GetTileInDirection(originTile, Direction.SouthWest);

                if (w?.Type != TileType.Dirt && sw?.Type != TileType.Dirt)
                {
                    return (false, 0);
                }

                if (w?.Type != TileType.Dirt)
                {
                    return (true, 2);
                }
                else if (sw?.Type != TileType.Dirt)
                {
                    return (true, -2);
                }
                else
                {
                    return (true, 0);
                }
            }
            else if (diffY == -44 && diffX == -87) // 2 range, NorthWest
            {
                Tile w = GetTileInDirection(originTile, Direction.West);
                Tile nw = GetTileInDirection(originTile, Direction.NorthWest);

                if (w?.Type != TileType.Dirt && nw?.Type != TileType.Dirt)
                {
                    return (false, 0);
                }

                if (w?.Type != TileType.Dirt)
                {
                    return (true, -2);
                }
                else if (nw?.Type != TileType.Dirt)
                {
                    return (true, 2);
                }
                else
                {
                    return (true, 0);
                }
            }
            else if (diffY == 88 && diffX == 174) // 4 range, SouthEast
            {
                Tile e = GetTileInDirection(originTile, Direction.East);
                Tile se = GetTileInDirection(originTile, Direction.SouthEast);
                Tile eFar = GetTileInDirection(destinationTile, Direction.NorthWest);
                Tile seFar = GetTileInDirection(destinationTile, Direction.West);

                if ((e?.Type == TileType.Dirt && se?.Type == TileType.Dirt) || (eFar?.Type == TileType.Dirt && seFar?.Type == TileType.Dirt))
                {
                    if (e?.Type == TileType.Dirt)
                    {
                        return (true, -2);
                    }
                    else
                    {
                        return (true, 2);
                    }
                }
                else
                {
                    return (false, 0);
                }
            }
            else if (diffY == -88 && diffX == 174) // 4 range, NorthEast
            {
                Tile e = GetTileInDirection(originTile, Direction.East);
                Tile ne = GetTileInDirection(originTile, Direction.NorthEast);
                Tile eFar = GetTileInDirection(destinationTile, Direction.SouthWest);
                Tile neFar = GetTileInDirection(destinationTile, Direction.West);

                if ((e?.Type == TileType.Dirt && ne?.Type == TileType.Dirt) || (eFar?.Type == TileType.Dirt && neFar?.Type == TileType.Dirt))
                {
                    if (e?.Type == TileType.Dirt)
                    {
                        return (true, 2);
                    }
                    else
                    {
                        return (true, -2);
                    }
                }
                else
                {
                    return (false, 0);
                }
            }
            else if (diffY == 88 && diffX == -174) // 4 range, SouthWest
            {
                Tile w = GetTileInDirection(originTile, Direction.West);
                Tile sw = GetTileInDirection(originTile, Direction.SouthWest);
                Tile wFar = GetTileInDirection(destinationTile, Direction.NorthEast);
                Tile swFar = GetTileInDirection(destinationTile, Direction.East);

                if ((w?.Type == TileType.Dirt && sw?.Type == TileType.Dirt) || (wFar?.Type == TileType.Dirt && swFar?.Type == TileType.Dirt))
                {
                    if (w?.Type == TileType.Dirt)
                    {
                        return (true, -2);
                    }
                    else
                    {
                        return (true, 2);
                    }
                }
                else
                {
                    return (false, 0);
                }
            }
            else if (diffY == -88 && diffX == -174) // 4 range, NorthWest
            {
                Tile w = GetTileInDirection(originTile, Direction.West);
                Tile nw = GetTileInDirection(originTile, Direction.NorthWest);
                Tile wFar = GetTileInDirection(destinationTile, Direction.SouthEast);
                Tile nwFar = GetTileInDirection(destinationTile, Direction.East);

                if ((w?.Type == TileType.Dirt && nw?.Type == TileType.Dirt) || (wFar?.Type == TileType.Dirt && nwFar?.Type == TileType.Dirt))
                {
                    if (w?.Type == TileType.Dirt)
                    {
                        return (true, 2);
                    }
                    else
                    {
                        return (true, -2);
                    }
                }
                else
                {
                    return (false, 0);
                }
            }
            else
            {
                return (true, 0);
            }
        }

        private Direction GetNextDirectionClockwise(Direction lastDirection)
        {
            switch (lastDirection)
            {
                case Direction.NorthEast: return Direction.East;
                case Direction.East: return Direction.SouthEast;
                case Direction.SouthEast: return Direction.SouthWest;
                case Direction.SouthWest: return Direction.West;
                case Direction.West: return Direction.NorthWest;
                case Direction.NorthWest: return Direction.NorthEast;
                default: return Direction.Default;
            }
        }
    }
}