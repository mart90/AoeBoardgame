using System.Collections.Generic;

namespace AoeBoardgame
{
    partial class PathFinder
    {
        private class Node
        {
            public readonly int X;
            public readonly int Y;
            public readonly List<Node> NodesVisited;

            private readonly Direction _lastDirection;

            public bool PathStopsHere { get; set; }

            public Node(int x, int y, Node lastNode = null, Direction lastDirection = Direction.Default)
            {
                X = x;
                Y = y;
                NodesVisited = new List<Node>();
                _lastDirection = lastDirection;

                if (lastNode != null)
                {
                    NodesVisited.AddRange(lastNode.NodesVisited);
                    NodesVisited.Add(lastNode);
                }
            }

            public List<Direction> GetNewDirections()
            {
                switch (_lastDirection)
                {
                    case Direction.NorthEast:
                        return new List<Direction>
                        {
                            Direction.NorthEast,
                            Direction.NorthWest,
                            Direction.East
                        };
                    case Direction.East:
                        return new List<Direction>
                        {
                            Direction.East,
                            Direction.NorthEast,
                            Direction.SouthEast
                        };
                    case Direction.SouthEast:
                        return new List<Direction>
                        {
                            Direction.SouthEast,
                            Direction.East,
                            Direction.SouthWest
                        };
                    case Direction.SouthWest:
                        return new List<Direction>
                        {
                            Direction.SouthWest,
                            Direction.West,
                            Direction.SouthEast
                        };
                    case Direction.West:
                        return new List<Direction>
                        {
                            Direction.West,
                            Direction.NorthWest,
                            Direction.SouthWest
                        };
                    case Direction.NorthWest:
                        return new List<Direction>
                        {
                            Direction.NorthWest,
                            Direction.West,
                            Direction.NorthEast
                        };
                    default:
                        return new List<Direction>
                        {
                            Direction.NorthEast,
                            Direction.East,
                            Direction.SouthEast,
                            Direction.SouthWest,
                            Direction.West,
                            Direction.NorthWest
                        };
                }
            }
        }
    }
}