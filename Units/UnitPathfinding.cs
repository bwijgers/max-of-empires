using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Units
{
    abstract partial class Unit:GameObjects.GameObject
    {
        private List<PathToTile> shortestPaths;
        private Point target;

        private class PathToTile
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
        }

        /// <summary>
        /// adds the paths to the tiles surrounding the specified tile to the list.
        /// </summary>
        /// <param name="startPath">PathToTile to the tile from which you want the neighboring tiles</param>
        private void AddSurroundingTiles(PathToTile startPath, List<PathToTile> newPaths)
        {
            List<PathToTile> returnList = new List<PathToTile>();

            //Creates list of points and fills it with the points surrounding the starttile
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
                    //forms new path to each point
                    int startPathLength = startPath.path == null ? 0 : startPath.path.Length;
                    Point[] pathAsPoints = new Point[startPathLength + 1];
                    startPath.path?.CopyTo(pathAsPoints, 0);
                    pathAsPoints[startPathLength] = p;
                    int cost = tile.Cost(this) + startPath.cost;
                    PathToTile newPathToTile = new PathToTile(p, pathAsPoints, cost);
                    PathToTile shortestPathToTile = shortestPaths.Find(path => path.target.Equals(p));
                    
                    //if the current found path is shorter than an existent path, overrides it.
                    if (shortestPathToTile != null)
                    {
                        if (cost < shortestPathToTile.cost)
                        {
                            newPaths.Remove(shortestPathToTile);
                            newPaths.Add(newPathToTile);
                            shortestPaths.Add(newPathToTile);
                        }
                    }
                    //if there is no known path to the specified point, adds this path to the list.
                    else
                    {
                        newPaths.Add(newPathToTile);
                        shortestPaths.Add(newPathToTile);
                    }
                }
            }
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
        /// determines the furthest point it can move to.
        /// </summary>
        /// <returns>the furthest point it can move to</returns>
        public Point MoveTowardsTarget()
        {
            if(((GameWorld as Grid)[target] as Tile).Occupied)
            {
                target = GridPos;
            }
            if (!(GridPos == target))
            {
                GeneratePaths(GridPos);
                Point[] Path = ShortestPath(target).path;
                Point temporalTarget = GridPos;
                int i = 0;
                bool foundPath = false;
                Point reachableTarget = GridPos;
                while (!foundPath)
                {
                    //if it can't move at all
                    if(!(ShortestPath(Path[i]).cost <= MovesLeft))
                    {
                        return GridPos;
                    }
                    //if it can move until the end of the path
                    if (ShortestPath(Path[i]).cost <= MovesLeft && i+1 == Path.Length)
                    {
                        foundPath = true;
                        reachableTarget = Path[i];
                    }
                    //if it can only complete a part of the path
                    else if (ShortestPath(Path[i]).cost <= MovesLeft && !(ShortestPath(Path[i + 1]).cost <= MovesLeft))
                    {
                        foundPath = true;
                        reachableTarget = Path[i];
                    }
                    //take one more "step" on the path
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
        /// Returns the positions of all reachable tiles.
        /// </summary>
        /// <returns></returns>
        public Point[] ReachableTiles()
        {
            GeneratePaths(GridPos);
            List<PathToTile> reachablePaths = shortestPaths.FindAll(path => path.cost <= MovesLeft);
            Point[] retVal = new Point[reachablePaths.Count];
            for (int i = 0; i < reachablePaths.Count; i++)
            {
                retVal[i] = reachablePaths[i].target;
            }
            return retVal;
        }


        /// <summary>
        /// Target location.
        /// </summary>
        public Point setTarget
        {
            set
            {
                if ((GameWorld as Grid).IsInGrid(value))
                {
                    target = value;
                }
            }
        }


        /// <summary>
        /// Returns the PathToTile to the specified coördinates.
        /// </summary>
        /// <param name="p">Point to which you need a path</param>
        /// <returns></returns>
        private PathToTile ShortestPath(Point p)
        {
            return shortestPaths.Find(path => path.target.Equals(p));
        }
    }
}
