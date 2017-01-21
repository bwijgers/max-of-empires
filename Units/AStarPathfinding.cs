using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaxOfEmpires.Units
{
    partial class Pathfinding
    {
        public static PathToTile AStar(Unit movingUnit, Point startingPoint, Point targetPoint)
        {
            // All evaluated points
            List<Point> closedSet = new List<Point>();

            // All discovered points to be evaluated
            List<Point> openSet = new List<Point>();
            openSet.Add(startingPoint);

            // Point value where point key came from
            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

            // For each node, the cost to get there from the start
            Dictionary<Point, int> distanceToTile = new Dictionary<Point, int>();
            // The cost to get to the start is always 0 ;)
            distanceToTile[startingPoint] = 0;

            // The estimated cost to get to the target
            Dictionary<Point, int> distanceToTarget = new Dictionary<Point, int>();
            distanceToTarget[startingPoint] = DistanceTo(startingPoint, targetPoint);

            // Gameworld to know if neighbors exist, to get costs of tiles, etc.
            Grid grid = movingUnit.GameWorld as Grid;

            // While we still have points to visit
            while (openSet.Count > 0)
            {
                Point probablyClosest = GetProbablyClosest(openSet, distanceToTarget);
                if (probablyClosest.Equals(targetPoint))
                {
                    return ReconstructPath(movingUnit, cameFrom, probablyClosest, grid);
                }

                // We've evaluated this point now
                openSet.Remove(probablyClosest);
                closedSet.Add(probablyClosest);

                List<Point> neighbors = GetNeighbors(grid, probablyClosest);
                foreach (Point neighbor in neighbors)
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue; // We've already evaluated this matey
                    }

                    // Distance from start to current neighbor
                    if (!distanceToTile.ContainsKey(probablyClosest))
                    {
                        distanceToTile[probablyClosest] = int.MaxValue;
                    }

                    int distanceToTileCandidate = distanceToTile[probablyClosest] + (grid[neighbor] as Tile).Cost(movingUnit);
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (distanceToTileCandidate >= distanceToTile[neighbor])
                    {
                        continue; // We had a path and this new one is not better
                    }

                    // Best path to this tile so far :)
                    cameFrom[neighbor] = probablyClosest;
                    distanceToTile[neighbor] = distanceToTileCandidate;
                    distanceToTarget[neighbor] = distanceToTile[neighbor] + DistanceTo(neighbor, targetPoint);
                }
            }

            // We couldn't find anything :/
            return null;
        }

        public static int DistanceTo(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private static List<Point> GetNeighbors(Grid world, Point p)
        {
            List<Point> neighbors = new List<Point>();

            if (world.IsInGrid(p + new Point(1, 0)))
                neighbors.Add(p + new Point(1, 0));

            if (world.IsInGrid(p + new Point(-1, 0)))
                neighbors.Add(p + new Point(-1, 0));

            if (world.IsInGrid(p + new Point(0, 1)))
                neighbors.Add(p + new Point(0, 1));

            if (world.IsInGrid(p + new Point(0, -1)))
                neighbors.Add(p + new Point(0, -1));

            return neighbors;
        }

        private static Point GetProbablyClosest(List<Point> openSet, Dictionary<Point, int> distanceToTarget)
        {
            int lowestDistance = int.MaxValue;
            Point closestPoint = new Point(-1, -1);

            foreach (Point p in openSet)
            {
                if (distanceToTarget.ContainsKey(p) && lowestDistance > distanceToTarget[p])
                {
                    lowestDistance = distanceToTarget[p];
                    closestPoint = p;
                }
            }

            if (closestPoint.Equals(new Point(-1, -1)))
            {
                throw new Exception("Hey we didn't find anything that's close mate.");
            }

            return closestPoint;
        }

        private static PathToTile ReconstructPath(Unit movingUnit, Dictionary<Point, Point> cameFrom, Point probablyClosest, Grid world)
        {
            Point targetPoint = probablyClosest;
            List<Point> path = new List<Point>();
            int cost = 0;

            while (cameFrom.ContainsKey(probablyClosest) && probablyClosest != movingUnit.PositionInGrid)
            {
                path.Add(probablyClosest);
                cost += (world[probablyClosest] as Tile).Cost(movingUnit);
                probablyClosest = cameFrom[probablyClosest];
                //cost += 1;
            }

            path.Reverse();
            PathToTile pathToTile = new PathToTile(targetPoint, path.ToArray(), cost);
            return pathToTile;
        }
    }
}
