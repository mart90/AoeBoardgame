using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<Tile> GetAllTilesInRange(int originTileId, int range)
        {
            var nodes = new List<Node>();
            Node originNode = GetNodeByTileId(originTileId);

            _nodeScores[originNode.Y][originNode.X] = 0;
            _nodeQueue.Enqueue(originNode);

            while (_nodeQueue.Count > 0)
            {
                Node currentNode = _nodeQueue.Dequeue();

                if (currentNode.NodesVisited.Count == range)
                {
                    return ConvertNodesToTiles(nodes);
                }

                IEnumerable<Node> children = GetAccessibleChildren(currentNode);

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

        // Breadth-first search
        public IEnumerable<Tile> GetOptimalPath(int originTileId, int destinationTileId)
        {
            Node originNode = GetNodeByTileId(originTileId);
            Node destinationNode = GetNodeByTileId(destinationTileId);

            _nodeScores[originNode.Y][originNode.X] = 0;
            _nodeQueue.Enqueue(originNode);

            while (_nodeQueue.Count > 0)
            {
                Node currentNode = _nodeQueue.Dequeue();
                IEnumerable<Node> children = GetAccessibleChildren(currentNode);

                foreach (var child in children)
                {
                    if (child.X == destinationNode.X && child.Y == destinationNode.Y)
                    {
                        var bestPath = child.NodesVisited;
                        bestPath.RemoveAt(0);
                        return ConvertNodesToTiles(bestPath);
                    }

                    int childScore = _nodeScores[child.Y][child.X];
                    if (childScore != -1 && childScore <= child.NodesVisited.Count) 
                    {
                        // There is another path that reached this tile in less (or an equal amount of) steps. Discard
                        continue;
                    }

                    _nodeScores[child.Y][child.X] = child.NodesVisited.Count;
                    _nodeQueue.Enqueue(child);
                }
            }
            // No path found
            return null; 
        }

        private IEnumerable<Tile> ConvertNodesToTiles(IEnumerable<Node> nodes)
        {
            return nodes.Select(e => _map.GetTileByCoordinates(e.X, e.Y)).ToList();
        }

        private IEnumerable<Node> GetAccessibleChildren(Node node)
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

                int childTileId = child.Y * _map.Width + child.X;
                if (_map.Tiles[childTileId].IsAccessible())
                {
                    accessibleChildren.Add(child);
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

        private Node GetNodeByTileId(int id)
        {
            var x = id % _map.Width;
            var y = id / _map.Width;
            return new Node(x, y);
        }
    }
}