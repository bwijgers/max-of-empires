using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameObjects
{
    /// <summary>
    /// A grid of GameObjects. Similar to GameObjectList, except that the orientation of the objects matters in this case.
    /// </summary>
    /// <see cref="GameObjectList"/>
    class GameObjectGrid : GameObject
    {
        /// <summary>
        /// Used by this class for methods that need to be called on every element.
        /// </summary>
        /// <param name="obj">The GameObject the method is called upon.</param>
        /// <param name="x">The x-position in the grid of this GameObject.</param>
        /// <param name="y">The y-position in the grid of this GameObject.</param>

        public delegate void GridFunc(GameObject obj);

        /// <summary>
        /// The grid. A digital-- Wait, I did this already.
        /// </summary>
        private GameObject[,] grid;

        /// <summary>
        /// Creates a new GameObjectGrid.
        /// </summary>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        /// <param name="id">The name of the grid. Empty by default.</param>
        public GameObjectGrid(int width, int height, string id = "") : base (id)
        {
            grid = new GameObject[width, height];
        }

        /// <see cref="GameObject.Draw(GameTime, SpriteBatch)"/>
        public override void Draw(GameTime time, SpriteBatch s)
        {
            // Draw every element on the grid if it is not null.
            ForEach(obj => obj?.Draw(time, s));
        }

        /// <summary>
        /// Calles a function for every element in the grid.
        /// </summary>
        /// <param name="func">The function to call for each element.</param>
        public void ForEach(GridFunc func)
        {
            // Make sure we gotta do something before we try doing something.
            if (func == null || func.GetInvocationList().Length == 0)
                return;

            // Do whatever is desired for every element in this grid.
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    func.Invoke(grid[x, y]);
                }
            }
        }

        /// <see cref="GameObject.HandleInput(InputHelper, KeyManager)"/>
        public override void HandleInput(InputHelper helper, KeyManager keyManager)
        {
            // Handle input for every element in grid
            ForEach(obj => obj?.HandleInput(helper, keyManager));
        }

        /// <summary>
        /// Checks whether a certain position exists within this GameObjectGrid.
        /// </summary>
        /// <param name="p">The position to check for.</param>
        /// <returns>True if the position exists within the grid, false otherwise.</returns>
        public bool IsInGrid(Point p)
        {
            return IsInGrid(p.X, p.Y);
        }

        /// <summary>
        /// Checks whether a certain position exists within this GameObjectGrid.
        /// </summary>
        /// <param name="x">The x-coordinate of the position to check for.</param>
        /// <param name="y">The y-coordinate of the position to check for.</param>
        /// <returns>True if the position exists within the grid, false otherwise.</returns>
        public bool IsInGrid(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>
        /// Gets the object at the specified position, or null if there is no such object.
        /// </summary>
        /// <param name="x">The X-position to check at.</param>
        /// <param name="y">The y-position to check at.</param>
        /// <returns>The GameObject at the specified position, which can be null, or null if the position was out of the grid bounds.</returns>
        public GameObject ObjectAt(int x, int y)
        {
            // If the coords are in bounds, return the object at the coords.
            if (IsInGrid(x, y))
                return grid[x, y];

            // Otherwise, return null
            return null;
        }

        /// <see cref="GameObject.Reset"/>
        public override void Reset()
        {
            // Reset every element in the grid
            ForEach(obj => obj?.Reset());
        }

        /// <see cref="GameObject.TurnUpdate(uint, bool)"/>
        public override void TurnUpdate(uint turn, Player player)
        {
            // Just TurnUpdate everything in the grid
            ForEach(obj => obj?.TurnUpdate(turn, player));
        }

        /// <see cref="GameObject.Update(GameTime)"/>
        public override void Update(GameTime time)
        {
            // Just update everything in the grid
            ForEach(obj => obj?.Update(time));
        }

        /// <summary>
        /// The height of this grid, being the amount of elements in the y-direction.
        /// </summary>
        public int Height => grid.GetLength(1);

        /// <summary>
        /// The Width and Height of this grid.
        /// </summary>
        public Point Size => new Point(Width, Height);

        /// <summary>
        /// The width of this grid, being the amount of elements in the x-direction.
        /// </summary>
        public int Width => grid.GetLength(0);

        /// <summary>
        /// Gets the GameObject at the specified position of this GameObjectGrid.
        /// </summary>
        /// <param name="x">The x-coord of the position.</param>
        /// <param name="y">The y-coord of the position.</param>
        /// <returns>The GameObject at this position, or null if there is no such GameObject.</returns>
        public GameObject this[int x, int y]
        {
            get
            {
                return ObjectAt(x, y);
            }
            protected set
            {
                // Only set the GameObject at the position if it is a valid position.
                if (IsInGrid(x, y))
                {
                    grid[x, y] = value;

                    // Also set the parent of the value to this GameObjectGrid
                    value.Parent = this;
                }
            }
        }

        /// <summary>
        /// Gets the GameObject at the specified position of this GameObjectGrid.
        /// </summary>
        /// <param name="p">The position.</param>
        /// <returns>The GameObject at this position, or null if there is no such GameObject.</returns>
        public GameObject this[Point p]
        {
            get
            {
                // We already have such a nice indexer; let's use that c:
                return this[p.X, p.Y];
            }
            protected set
            {
                // Same as with the get.
                this[p.X, p.Y] = value;
            }
        }
    }
}
