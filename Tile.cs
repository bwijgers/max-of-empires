using MaxOfEmpires.GameObjects;
using MaxOfEmpires.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui;

namespace MaxOfEmpires
{
    class Tile : GameObjectDrawable
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
        /// Whether certain overlays should be drawn.
        /// </summary>
        private bool overlayAttack, overlayWalk;

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
            position = new Vector2(x * 32, y * 32);
            overlayAttack = overlayWalk = false;
        }

        /// <summary>
        /// The movement cost for a specified Unit to move to this Tile.
        /// </summary>
        /// <param name="unit">The Unit for which you want to know the movement cost.</param>
        /// <returns>The movement cost on this Tile for the specified Unit.</returns>
        public int Cost(Unit unit)
        {
            return 1;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            terrain.Draw(DrawPosition.ToPoint(), s);
            building?.Draw(time, s);
            unit?.Draw(time, s);

            // Draw a walking overlay if it should be drawn
            if (overlayWalk)
            {
                DrawingHelper.Instance.DrawRectangle(s, Bounds, new Color(0x00, 0x00, 0xFF, 0x88));
            }

            // Draw an attacking overlay if it should be drawn
            if (overlayAttack)
            {
                DrawingHelper.Instance.DrawRectangle(s, Bounds, new Color(0xFF, 0x00, 0x00, 0x88));
            }
        }

        /// <summary>
        /// Whether or not this Tile is passable for a certain unit. 
        /// </summary>
        /// <param name="unit">The Unit for which you want to know if this Tile is passable.</param>
        /// <returns>True if the Unit can pass through this Tile, false otherwise.</returns>
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
                unit.PositionInGrid = new Point(x, y);
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

        public override void Update(GameTime time)
        {
            building?.Update(time);
            unit?.Update(time);
        }

        /// <summary>
        /// The Building built on this Tile.
        /// </summary>
        public Building Building => building;

        /// <summary>
        /// Whether there is a Building on this Tile.
        /// </summary>
        public bool BuiltOn => building != null;

        /// <summary>
        /// The position in the Grid of this Tile.
        /// </summary>
        public Point GridPos => new Point(x, y);

        /// <summary>
        /// Whether there is a Unit on this Tile.
        /// </summary>
        public bool Occupied => Unit != null;

        /// <summary>
        /// True when the attacking overlay on this tile should be shown, false otherwise.
        /// </summary>
        public bool OverlayAttack
        {
            get
            {
                return overlayAttack;
            }
            set
            {
                overlayAttack = value;
            }
        }

        /// <summary>
        /// True when the walking overlay on this tile should be shown, false otherwise.
        /// </summary>
        public bool OverlayWalk
        {
            get
            {
                return overlayWalk;
            }
            set
            {
                overlayWalk = value;
            }
        }

        public override Vector2 Size
        {
            get
            {
                return new Vector2(32, 32);
            }
        }

        /// <summary>
        /// The Terrain of this Tile.
        /// </summary>
        public Terrain Terrain => terrain;

        /// <summary>
        /// The Unit occupying this Tile.
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
