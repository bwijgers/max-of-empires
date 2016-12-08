using MaxOfEmpires.GameObjects;
using MaxOfEmpires.Units;
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
        /// The current Building on this Tile. Can be null.
        /// </summary>
        private Building building;

        /// <summary>
        /// The Terrain of this Tile.
        /// </summary>
        private Terrain terrain;

        /// <summary>
        /// The current Unit on this Tile. Can be null.
        /// </summary>
        private Unit unit;

        /// <summary>
        /// The x and y positions of this Tile in the containing Grid.
        /// </summary>
        private int x, y;

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
        /// <summary>
        /// returns the movement cost for a specified unit to move to this tile.
        /// </summary>
        /// <param name="unit">the unit for whom you want to know the movement cost</param>
        /// <returns></returns>
        public int Cost(Unit unit)
        {
            return 1;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            terrain.Draw(x, y, s);
            building?.Draw(time, s);
            unit?.Draw(time, s);
        }

        /// <summary>
        /// returns wether or not this tile is passable for a certain unit
        /// </summary>
        /// <param name="unit">the unit for whom you want to know if this tile is passable</param>
        /// <returns></returns>
        public bool Passable(Unit unit)
        {
            return !Occupied || Unit.Owner == unit.Owner;
        }

        /// <summary>
        /// Sets a Unit on this tile. Also updates that Unit's GridPos to this Tile's position.
        /// </summary>
        /// <param name="u">The Unit to set on this Tile.</param>
        public void SetUnit(Unit u)
        {
            // Check to make sure the unit is not overriding another unit.
            if (Occupied && u != null)
                return;

            // Set the unit 
            unit = u;

            // Set the unit's position and parent, if it is not null
            if(unit != null)
            {
                unit.GridPos = new Point(x, y);
                unit.Parent = this;
            }
        }

        public override void TurnUpdate(uint turn, bool player)
        {
            // Update the Unit at this position if it exists.
            if (Occupied)
                Unit.TurnUpdate(turn, player);

            // Update the Building at this position if it exists.
            if (BuiltOn)
                Building.TurnUpdate(turn, player);
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
        public Unit Unit
        {
            get
            {
                return unit;
            }
        }
    }
}
