using System.Collections.Generic;

namespace AoeBoardgame
{
    partial class PathFinder
    {
        private Map _map;
        private List<PathFinderNode> _nodes;

        public PathFinder(Map map)
        {
            _map = map;
            _nodes = new List<PathFinderNode>();

            for (int i = 0; i < _map.Tiles.Count; i++)
            {
                _nodes.Add(new PathFinderNode(
                    i,
                    i % _map.Width,
                    _map.Height - i / _map.Width
                ));
            }
        }

        private enum Direction
        {
            NorthWest,
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West
        }

        private List<PathFinderNode> GetAccessibleChildren(PathFinderNode node)
        {
            var accessibleChildren = new List<PathFinderNode>();
            
            return accessibleChildren;
        }

        public void FindOptimalPath(int originTileId, int destinationTileId)
        {
            var originNode = _nodes.Find(e => e.Id == originTileId);
            var destinationNode = _nodes.Find(e => e.Id == destinationTileId);

            originNode.DistanceToNode(destinationNode);

        }
    }
}