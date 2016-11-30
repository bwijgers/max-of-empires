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
        private delegate void GridFunc(GameObject obj, int x, int y);

        /// <summary>
        /// The grid. A digital-- Wait, I did this already.
        /// </summary>
        protected GameObject[,] grid;

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
            ForEach((obj, x, y) => obj?.Draw(time, s));
        }

        /// <summary>
        /// Calles a function for every element in the grid.
        /// </summary>
        /// <param name="func">The function to call for each element.</param>
        private void ForEach(GridFunc func)
        {
            // Make sure we gotta do something before we try doing something.
            if (func == null || func.GetInvocationList().Length == 0)
                return;

            // Do whatever is desired for every element in this grid.
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    func.Invoke(grid[x, y], x, y);
                }
            }
        }

        /// <see cref="GameObject.HandleInput(InputHelper, KeyManager)"/>
        public override void HandleInput(InputHelper helper, KeyManager keyManager)
        {
            // Handle input for every element in grid
            ForEach((obj, x, y) => obj?.HandleInput(helper, keyManager));
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
            if (x >= 0 && y >= 0 && x < Width && y < Height)
                return grid[x, y];

            // Otherwise, return null
            return null;
        }

        /// <see cref="GameObject.Reset"/>
        public override void Reset()
        {
            // Reset every element in the grid
            ForEach((obj, x, y) => obj?.Reset());
        }

        /// <summary>
        /// Sets an element in the grid. Returns a success code.
        /// </summary>
        /// <param name="obj">The object to place in the grid.</param>
        /// <param name="x">The x-position to place the object in.</param>
        /// <param name="y">The y-position to place the object in.</param>
        /// <param name="force">Whether to force placement; if true, ignores whatever is in the position already and overwrites it.</param>
        /// <returns>True if this object was placed at the specified position, false otherwise.</returns>
        public bool SetElementInGrid(GameObject obj, int x, int y, bool force = false)
        {
            // If we can't place this object here, return false
            if (!force && grid[x, y] != null)
                return false;

            // Place the object here and return true
            grid[x, y] = obj;
            return true;
        }

        /// <see cref="GameObject.Update(GameTime)"/>
        public override void Update(GameTime time)
        {
            // Just update everything in the grid
            ForEach((obj, x, y) => obj?.Update(time));
        }

        /// <see cref="GameObject.TurnUpdate(uint, bool)"/>
        public override void TurnUpdate(uint turn, bool player)
        {
            // Just TurnUpdate everything in the grid
            ForEach((obj, x, y) => obj?.TurnUpdate(turn, player));
        }

        /// <summary>
        /// The width of this grid, being the amount of elements in the x-direction.
        /// </summary>
        public int Width => grid.GetLength(0);

        /// <summary>
        /// The height of this grid, being the amount of elements in the y-direction.
        /// </summary>
        public int Height => grid.GetLength(1);
    }
}
