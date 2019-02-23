using System.Collections.Generic;
using System.Management.Instrumentation;

namespace AoeBoardgame
{
    partial class PathFinder
    {
        private class PathFinderNode
        {
            public readonly int Id;
            public readonly int X;
            public readonly int Y;

            public PathFinderNode(int id, int x, int y)
            {
                Id = id;
                X = x;
                Y = y;
            }

            public int DistanceToNode(PathFinderNode destinationNode)
            {
                if (destinationNode.Y == Y && destinationNode.X == X)
                {
                    return 0;
                }

                int x = X, y = Y;
                int stepsTaken = 0;

                var direction = FindDirectionToNode(destinationNode, x, y);
                while (x != destinationNode.X && y != destinationNode.Y)
                {
                    TakeStep(direction, ref x, ref y);
                    stepsTaken++;
                }

                direction = FindDirectionToNode(destinationNode, x, y);
                while (x != destinationNode.X || y != destinationNode.Y)
                {
                    TakeStep(direction, ref x, ref y);
                    stepsTaken++;
                }

                return stepsTaken;
            }

            private static Direction FindDirectionToNode(PathFinderNode node, int originX, int originY)
            {
                if (node.Y == originY)
                {
                    return node.X > originX ? Direction.East : Direction.West;
                }

                if (node.Y < originY)
                {
                    return node.X == originX ? Direction.South :
                        node.X > originX ? Direction.SouthEast : Direction.SouthWest;
                }

                // node.Y > y
                return node.X == originX ? Direction.North :
                    node.X > originX ? Direction.NorthEast : Direction.NorthWest;
            }

            private static void TakeStep(Direction direction, ref int x, ref int y)
            {
                bool evenRow = y % 2 == 0;

                switch (direction)
                {
                    case Direction.North:
                        y++;
                        break;
                    case Direction.NorthEast:
                        y++;
                        if (!evenRow)
                            x++;
                        break;
                    case Direction.East:
                        x++;
                        break;
                    case Direction.SouthEast:
                        y--;
                        if (!evenRow)
                            x++;
                        break;
                    case Direction.South:
                        y--;
                        break;
                    case Direction.SouthWest:
                        y--;
                        if (evenRow)
                            x--;
                        break;
                    case Direction.West:
                        x--;
                        break;
                    case Direction.NorthWest:
                        y++;
                        if (evenRow)
                            x--;
                        break;
                }
            }
        }
    }
}