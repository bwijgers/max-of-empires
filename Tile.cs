using MaxOfEmpires.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires
{
    class Tile : GameObject
    {
        /// <summary>
        /// The x and y positions of this Tile in the containing Grid.
        /// </summary>
        private int x, y;

        /// <summary>
        /// The Terrain of this Tile.
        /// </summary>
        private Terrain terrain;

        /// <summary>
        /// The current Unit on this Tile. Can be null.
        /// </summary>
        private Unit unit;

        /// <summary>
        /// The current Building on this Tile. Can be null.
        /// </summary>
        private Building building; 

        /// <summary>
        /// Creates a new Tile at a specified position with a specified Terrain.
        /// </summary>
        /// <param name="terrain">The Terrain that this Tile should have.</param>
        /// <param name="x">The x-coord of the position that this Tile should be at.</param>
        /// <param name="y">The y-coord of the position that this Tile should be at.</param>
        public Tile(Terrain terrain, int x, int y)
        {
            this.terrain = terrain;
            this.x = x;
            this.y = y;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            terrain.Draw(x, y, s);
        }

        /// <summary>
        /// Returns the Building built on this Tile.
        /// </summary>
        public Building Building => building;

        /// <summary>
        /// Checks whether there is a Building on this Tile.
        /// </summary>
        public bool BuiltOn => building != null;

        /// <summary>
        /// Checks whether there is a Unit on this Tile.
        /// </summary>
        public bool Occupied => Unit != null;

        /// <summary>
        /// Returns the Terrain of this Tile.
        /// </summary>
        public Terrain Terrain => terrain;

        /// <summary>
        /// Returns the Unit occupying this Tile.
        /// </summary>
        public Unit Unit => unit;
    }
}
