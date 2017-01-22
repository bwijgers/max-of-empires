using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MaxOfEmpires.Units
{
    abstract class Unit : GameObjectAnimated
    {
        /// <summary>
        /// Whether this unit has attacked. Units can only attack once.
        /// </summary>
        protected bool hasAttacked;

        /// <summary>
        /// The amount of squares this unit can still move.
        /// </summary>
        protected int movesLeft;

        /// <summary>
        /// The total amount of squares a unit can move each turn.
        /// </summary>
        protected int moveSpeed;

        public string id;

        protected Player owner; // 2 is false, 1 is true

        /// <summary>
        /// The target position for this Unit.
        /// </summary>
        private Point target;

        /// <summary>
        /// The x and y coords of this Unit. Used for drawing and moving.
        /// </summary>
        private int x, y;

        public Unit(int x, int y, Player owner, string id = "") : base(false)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.owner = owner;
            target = new Point(x, y);
        }

        /// <summary>
        /// Calculates the distance from this Unit's position to the specified position.
        /// </summary>
        /// <param name="x">The x-coord of the new position.</param>
        /// <param name="y">The y-coord of the new position.</param>
        /// <returns>The total distance between the specified position and this Unit's position.</returns>
        protected int DistanceTo(int x, int y)
        {
            int xDist = Math.Abs(x - this.x);
            int yDist = Math.Abs(y - this.y);
            return xDist + yDist;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            // Draw the Unit based on whether it still has moves left; gray if there are no moves left.
            DrawColor = HasMoved ? Color.Gray : Color.White;
            base.Draw(time, s);
        }

        /// <summary>
        /// Checks if this Unit can move to the specified position. Returns false if this Unit can't reach the specified position.
        /// Assumes the specified position is not occupied.
        /// </summary>
        /// <param name="x">The x-coord of the position to move to.</param>
        /// <param name="y">The y-coord of the position to move to.</param>
        /// <returns>True if the Unit moved to the position, false otherwise.</returns>
        public virtual bool Move(int x, int y)
        {
            // If we already moved, we can't move anymore. Something like that
            if (HasMoved)
            {
                return false;
            }

            // Get the distance to the specified position.
            int distance = Pathfinding.ShortestPath(this, new Point(x, y)).cost;

            // Check if we can move to this position. Decrements moves left as well if we can.
            if (distance <= movesLeft)
            {
                movesLeft -= distance;
                return true;
            }
            return false;
        }

        public virtual bool Passable(Terrain terrain)
        {
            return !(terrain == Terrain.Mountain || terrain == Terrain.Lake || terrain == Terrain.DesertMountain || terrain == Terrain.TundraMountain);
        }

        public override void TurnUpdate(uint turn, Player player)
        {
            if (owner == player)
            {
                movesLeft = moveSpeed;
            }
            ShouldAnimate = player == owner;
        }

        /// <summary>
        /// Whether this unit can still perform an action this turn. 
        /// </summary>
        public virtual bool HasAction => !HasMoved;

        /// <summary>
        /// Whether this Unit can still move this turn.
        /// </summary>
        public bool HasMoved
        {
            get
            {
                return MovesLeft <= 0;
            }
            set
            {
                if (value)
                    movesLeft = 0;
            }
        }

        /// <summary>
        /// The amount of tiles this Unit can still run this turn.
        /// </summary>
        public int MovesLeft
        {
            get
            {
                return movesLeft;
            }
            set
            {
                movesLeft = value;
            }
        }

        /// <summary>
        /// The amount of tiles this Unit can run each turn.
        /// </summary>
        public int MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
            set
            {
                moveSpeed = value;
            }
        }

        /// <summary>
        /// The owner of this Unit. True => player 1, false => player 2.
        /// </summary>
        public Player Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
            }
        }

        /// <summary>
        /// The position in the Grid this Unit occupies.
        /// </summary>
        public Point PositionInGrid
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                this.x = value.X;
                this.y = value.Y;
            }
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
