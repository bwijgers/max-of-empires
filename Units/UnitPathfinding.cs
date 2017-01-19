using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Units
{
    partial class Unit : GameObjects.GameObjectDrawable
    {
        private List<PathToTile> shortestPaths;
        private Point target;

        public class PathToTile
        {
            public Point target;
            public Point[] path;
            public int cost;

            public PathToTile(Point target, Point[] path, int cost)
            {
                this.target = target;
                this.path = path;
                this.cost = cost;
            }

            //I attempted to cause a preference for direct lines: as close as possible to a straight line
            //double[] line(Point origin, Point target)
            //{
            //    double[] returnDoubles = new double[3];
            //    if (origin.Y == target.Y)
            //    {
            //        returnDoubles[0] = 0;
            //        returnDoubles[1] = 1;
            //        returnDoubles[2] = origin.Y;
            //        return returnDoubles;
            //    }
            //    double slope = (target.X - origin.X) / (target.Y - origin.Y);
            //    returnDoubles[2] = target.Y + slope * -target.X;
            //    returnDoubles[1] = -1;
            //    returnDoubles[0] = slope;
            //    return returnDoubles;
            //}
            //double distancePointToLine(double[] line, Point point)
            //{
            //    double upperPartFraction = Math.Abs(line[0] * point.X + line[1] * point.Y + line[2]);
            //    double lowerPartFraction = Math.Sqrt(Math.Pow(line[0], 2) + Math.Pow(line[1], 2));
            //    return (upperPartFraction / lowerPartFraction);
            //}
            //double cumulativeDistance()
            //{
            //    double returnValue = 0;
            //    double[] totalLine = line(path[0], target);
            //    foreach(Point p in path)
            //    {
            //        returnValue += distancePointToLine(totalLine, p);
            //    }
            //    return returnValue;
            //}
            //public double CumulativeDistance
            //{
            //    get
            //    {
            //        return cumulativeDistance();
            //    }
            //}

        }

        /// <summary>
        /// Adds the paths to the tiles surrounding the specified tile to the list.
        /// </summary>
        /// <param name="startPath">PathToTile to the tile from which you want the neighboring tiles.</param>
        private void AddSurroundingTiles(PathToTile startPath, List<PathToTile> newPaths)
        {
            List<PathToTile> returnList = new List<PathToTile>();

            // Creates list of points and fills it with the points surrounding the starttile
            List<Point> surroundingPoints = new List<Point>();
            surroundingPoints.Add(startPath.target + new Point(1, 0));
            surroundingPoints.Add(startPath.target + new Point(0, 1));
            surroundingPoints.Add(startPath.target + new Point(-1, 0));
            surroundingPoints.Add(startPath.target + new Point(0, -1));
            foreach (Point p in surroundingPoints)
            {
                if (!(GameWorld as Grid).IsInGrid(p))
                {
                    continue;
                }

                Tile tile = (GameWorld as Grid)[p] as Tile;
                if (tile.Passable(this))
                {
                    // Forms new path to each point
                    int startPathLength = startPath.path == null ? 0 : startPath.path.Length;
                    Point[] pathAsPoints = new Point[startPathLength + 1];
                    startPath.path?.CopyTo(pathAsPoints, 0);
                    pathAsPoints[startPathLength] = p;
                    int cost = tile.Cost(this) + startPath.cost;
                    PathToTile newPathToTile = new PathToTile(p, pathAsPoints, cost);
                    PathToTile shortestPathToTile = shortestPaths.Find(path => path.target.Equals(p));
                    
                    // If the current found path is shorter than an existent path, overrides it.
                    if (shortestPathToTile != null)
                    {
                        if (cost < shortestPathToTile.cost)
                        {
                            shortestPaths.Remove(shortestPathToTile);
                            newPaths.Add(newPathToTile);
                            shortestPaths.Add(newPathToTile);
                        }
                        //else if(cost == shortestPathToTile.cost && newPathToTile.CumulativeDistance < shortestPathToTile.CumulativeDistance)
                        //{
                        //    shortestPaths.Remove(shortestPathToTile);
                        //    newPaths.Add(newPathToTile);
                        //    shortestPaths.Add(newPathToTile);
                        //}
                    }
                    // If there is no known path to the specified point, adds this path to the list.
                    else
                    {
                        newPaths.Add(newPathToTile);
                        shortestPaths.Add(newPathToTile);
                    }
                }
            }
            newPaths.Remove(startPath);
        }

        /// <summary>
        /// Clears the Unit's target position.
        /// </summary>
        public void ClearTargetPosition()
        {
            target = PositionInGrid;
        }

        /// <summary>
        /// Generates the list of shortest paths.
        /// </summary>
        /// <param name="startPosition">Position from which all paths are created.</param>
        public void GeneratePaths(Point startPosition)
        {
            List<PathToTile> newPaths;
            PathToTile startPath = new PathToTile(startPosition, new Point[0], 0);
            newPaths = new List<PathToTile>();
            shortestPaths = new List<PathToTile>();
            newPaths.Add(startPath);
            while (newPaths.Count > 0)
            {
                List<PathToTile> newNewPaths = new List<PathToTile>();
                newNewPaths.AddRange(newPaths);


                foreach (PathToTile p in newNewPaths)
                {
                    AddSurroundingTiles(p,newPaths);
                    newPaths.Remove(p);
                }
            }
        }

        /// <summary>
        /// Returns the path to the specified coördinates.
        /// </summary>
        /// <param name="p">Point to which you need a path</param>
        /// <returns></returns>
        public Point[] GetPath(Point p)
        {
            return ShortestPath(p).path;
        }

        /// <summary>
        /// Determines the furthest point it can move to.
        /// </summary>
        /// <returns>The furthest point it can move to</returns>
        public Point MoveTowardsTarget()
        {
            // If the target tile is occupied, unset target. 
            if(((GameWorld as Grid)[target] as Tile).Occupied)
            {
                ClearTargetPosition();
                return target;
            }

            if (PositionInGrid != target)
            {
                GeneratePaths(PositionInGrid);
                Point[] Path = ShortestPath(target).path;
                int i = 0;
                bool foundPath = false;
                Point reachableTarget = PositionInGrid;
                while (!foundPath)
                {
                    // If it can't move at all
                    if (ShortestPath(Path[i]).cost > MovesLeft)
                    {
                        return PositionInGrid;
                    }

                    // If it can move until the end of the path
                    if (ShortestPath(Path[i]).cost <= MovesLeft && i+1 == Path.Length)
                    {
                        foundPath = true;
                        reachableTarget = Path[i];
                    }

                    // If it can only complete a part of the path
                    else if (ShortestPath(Path[i]).cost <= MovesLeft && ShortestPath(Path[i + 1]).cost > MovesLeft)
                    {
                        foundPath = true;
                        reachableTarget = Path[i];
                    }

                    // Take one more "step" on the path
                    else
                    {
                        i++;
                    }
                }
                return reachableTarget;
            }
            else
            {
                return target;
            }
        }

        /// <summary>
        /// Gets the position of all the Tiles this Unit can reach with the moves left this turn.
        /// </summary>
        /// <returns>All reachable Tiles this turn/</returns>
        public Point[] ReachableTiles()
        {
            GeneratePaths(PositionInGrid);
            List<PathToTile> reachablePaths = shortestPaths.FindAll(path => path.cost <= MovesLeft);
            Point[] retVal = new Point[reachablePaths.Count];
            for (int i = 0; i < reachablePaths.Count; i++)
            {
                retVal[i] = reachablePaths[i].target;
            }
            return retVal;
        }

        /// <summary>
        /// Returns the PathToTile to the specified coördinates.
        /// </summary>
        /// <param name="p">Point to which you need a path.</param>
        /// <returns></returns>
        public PathToTile ShortestPath(Point p)
        {
            return shortestPaths.Find(path => path.target.Equals(p));
        }

        /// <summary>
        /// Target location.
        /// </summary>
        public Point TargetPosition
        {
            get
            {
                return target;
            }
            set
            {
                if ((GameWorld as Grid).IsInGrid(value))
                {
                    target = value;
                }
            }
        }
    }
}
