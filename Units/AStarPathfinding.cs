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
                else if (probablyClosest.Equals(new Point(-1, -1)))
                {
                    // Couldn't find a valid path
                    return null;
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
                        //distanceToTile[probablyClosest] = int.MaxValue;
                    }

                    int distanceToTileCandidate = distanceToTile[probablyClosest] + (grid[neighbor] as Tile).Cost(movingUnit);
                    if (distanceToTileCandidate < 0)
                    {
                        distanceToTileCandidate = int.MaxValue;
                    }
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
                    if (distanceToTarget[neighbor] < 0)
                        distanceToTarget[neighbor] = int.MaxValue;
                }
            }

            // We couldn't find anything :/
            return null;
        }

        public static int DistanceTo(Point a, Point b)
        {
            return (int)new Vector2(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y)).Length();
        }

        private static List<Point> GetNeighbors(Grid world, Point p)
        {
            // Create list of neighbors
            List<Point> neighbors = new List<Point>();

            // Add all neighbors that exist in the world
            if (world.IsInGrid(p + new Point(1, 0)))
                neighbors.Add(p + new Point(1, 0));

            if (world.IsInGrid(p + new Point(-1, 0)))
                neighbors.Add(p + new Point(-1, 0));

            if (world.IsInGrid(p + new Point(0, 1)))
                neighbors.Add(p + new Point(0, 1));

            if (world.IsInGrid(p + new Point(0, -1)))
                neighbors.Add(p + new Point(0, -1));

            // Return neighbors
            return neighbors;
        }

        private static Point GetProbablyClosest(List<Point> openSet, Dictionary<Point, int> distanceToTarget)
        {
            // Init lowest distance and closest point (to target)
            int lowestDistance = int.MaxValue;
            Point closestPoint = new Point(-1, -1);

            // Go through all points in the open set to see which is closest
            foreach (Point p in openSet)
            {
                if (distanceToTarget.ContainsKey(p) && lowestDistance > distanceToTarget[p])
                {
                    lowestDistance = distanceToTarget[p];
                    closestPoint = p;
                }
            }

            // Found no closest? Strange...
#if DEBUG
            if (closestPoint.Equals(new Point(-1, -1)))
            {
//                throw new Exception("Hey we didn't find anything that's close mate.");
            }
#endif
            // Return the point that we think is closest
            return closestPoint;
        }

        private static PathToTile ReconstructPath(Unit movingUnit, Dictionary<Point, Point> cameFrom, Point probablyClosest, Grid world)
        {
            Point targetPoint = probablyClosest;
            List<Point> path = new List<Point>();
            int cost = 0;

            // While there's still points in the path to pass over
            while (cameFrom.ContainsKey(probablyClosest) && probablyClosest != movingUnit.PositionInGrid)
            {
                // Add the closest tiles, from target to start
                path.Add(probablyClosest);
                cost += (world[probablyClosest] as Tile).Cost(movingUnit);
                probablyClosest = cameFrom[probablyClosest];
            }

            // And reverse the path, to make sure it's from the Unit to its target
            path.Reverse();
            PathToTile pathToTile = new PathToTile(targetPoint, path.ToArray(), cost);
            return pathToTile;
        }
    }
}
