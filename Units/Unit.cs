using MaxOfEmpires.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.Units
{
    abstract partial class Unit : GameObject
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

        private bool owner; // 2 is false, 1 is true

        /// <summary>
        /// This Unit's stats. 
        /// </summary>
        /// <see cref="Units.Stats"/>
        private Stats stats;

        /// <summary>
        /// The texture of this Unit.
        /// </summary>
        private Texture2D texture;

        private int x;
        private int y; // The x and y coords of this Unit. Used for drawing and moving.

        protected Unit(int x, int y, bool owner, string resName)
        {
            this.x = x;
            this.y = y;
            this.owner = owner;

            // Get the texture based on the player (blue for p1, red for p2)
            StringBuilder texName = new StringBuilder();
            texName.Append(@"FE-Sprites\").Append(resName).Append('_');
            texName.Append(owner ? "blue" : "red");

            // Load the Unit's texture based on the name supplied and the player controlling the unit.
            texture = AssetManager.Instance.getAsset<Texture2D>(texName.ToString()); 
        }

        /// <summary>
        /// Calculates the distance from this Unit's position to the specified position.
        /// </summary>
        /// <param name="x">The x-coord of the new position.</param>
        /// <param name="y">The y-coord of the new position.</param>
        /// <returns>The total distance between the specified position and this Unit's position.</returns>
        private int DistanceTo(int x, int y)
        {
            int xDist = Math.Abs(x - this.x);
            int yDist = Math.Abs(y - this.y);
            return xDist + yDist;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            Color drawColor = HasMoved ? Color.Gray : Color.White;
            s.Draw(texture, DrawPos, drawColor);
        }

        /// <summary>
        /// Gets all positions this Unit can walk to. Used for overlay and actually moving.
        /// </summary>
        /// <returns>All valid positions for this Unit to walk to within its walking distance.</returns>
        public Point[] GetWalkablePositions()
        {
            List<Point> retVal = new List<Point>();
            for (int y = MovesLeft; y >= -MovesLeft; --y)
            {
                // y is the amount of moves used to move along the y-axis. x must be the moves left. 
                int movesLeft = MovesLeft - Math.Abs(y);

                for (int x = -movesLeft; x < movesLeft + 1; ++x)
                {
                    int xWalkPos = x + GridPos.X;
                    int yWalkPos = y + GridPos.Y;
                    if ((GameWorld as GameObjectGrid).IsInGrid(xWalkPos, yWalkPos))
                    {
                        retVal.Add(new Point(xWalkPos, yWalkPos));
                    }
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Moves this Unit to the specified position. Returns false if this Unit can't reach the specified position.
        /// Assumes the specified position is not occupied.
        /// </summary>
        /// <param name="x">The x-coord of the position to move to.</param>
        /// <param name="y">The y-coord of the position to move to.</param>
        /// <returns>True if the Unit moved to the position, false otherwise.</returns>
        public bool Move(int x, int y)
        {
            // If we already moved, we can't move anymore. Something like that
            if (HasMoved)
            {
                return false;
            }

            // Get the distance to the specified position.
            int distance = ShortestPath(new Point(x,y)).cost;

            // Check if we can move to this position before actually just moving there. CanMoveTo decrements MovesLeft as well, if it is possible to move to the position.
            if(distance <= movesLeft)
            {
                this.x = x;
                this.y = y;
                movesLeft -= distance;
                return true;
            }
            return false;
        }

        public override void TurnUpdate(uint turn, bool player)
        {
            movesLeft = moveSpeed;
            hasAttacked = false;
        }

        /// <summary>
        /// The position to draw this Unit at. Returns the top-left-most pixel to draw at.
        /// </summary>
        public Vector2 DrawPos => new Vector2(x * texture.Width, y * texture.Height);

        /// <summary>
        /// The position in the Grid this Unit occupies.
        /// </summary>
        public Point GridPos
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
        /// Whether this unit can still perform an action this turn. 
        /// </summary>
        public bool HasAction => !HasAttacked || !HasMoved;

        /// <summary>
        /// Whether this Unit has attacked this turn.
        /// </summary>
        public bool HasAttacked => hasAttacked;

        /// <summary>
        /// Whether this Unit can still move this turn.
        /// </summary>
        public bool HasMoved => MovesLeft <= 0;

        /// <summary>
        /// The amount of tiles this Unit can still run this turn.
        /// </summary>
        public int MovesLeft => movesLeft;

        /// <summary>
        /// The amount of tiles this Unit can run each turn.
        /// </summary>
        public int MoveSpeed => moveSpeed;

        /// <summary>
        /// The owner of this Unit. True => player 1, false => player 2.
        /// </summary>
        public bool Owner => owner;

        /// <summary>
        /// The Stats of this Unit. 
        /// </summary>
        /// <see cref="Units.Stats"/>
        public Stats Stats => stats;
    }
}
