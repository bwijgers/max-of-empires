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
            target = new Point(x, y);

        // Get the texture based on the player (blue for p1, red for p2)
        StringBuilder texName = new StringBuilder();
            texName.Append(@"FE-Sprites\").Append(resName).Append('_');
            texName.Append(owner ? "blue" : "red");

            // Load the Unit's texture based on the name supplied and the player controlling the unit.
            texture = AssetManager.Instance.getAsset<Texture2D>(texName.ToString()); 
        }

        public void Attack(Point attackPos)
        {
            // Get the gameWorld and the Tile to attack a Unit on. 
            Grid gameWorld = GameWorld as Grid;
            Tile toAttack = gameWorld[attackPos] as Tile;

            // Check if the attacking tile actually has a Unit to attack. 
            // Also check if the Unit can actually attack the enemy from here.
            if (!toAttack.Occupied && IsInRange(attackPos))
            {
                return; // This should *never* happen. 
            }

            // Actually damage the enemy unit.
            Unit enemy = toAttack.Unit;
            DealDamage(enemy);

            // Don't do anything else if the enemy is dead. 
            if (enemy.IsDead)
                return;

            // Enemy can retaliate. Right?
            if (enemy.IsInRange(GridPos))
            {
                // Okay, retaliate. 
                enemy.DealDamage(this);
            }

            // Unit has attacked
            hasAttacked = true;
        }

        private void DealDamage(Unit enemy)
        {
            // Check if we hit at all
            int hitChance = stats.hit - enemy.stats.dodge;
            if (hitChance < 100)
            {
                double randDouble = MaxOfEmpires.Random.NextDouble();
                if (randDouble * 100 > hitChance)
                {
                    return; // we missed :c
                }
            }

            // We hit :D Damage the enemy
            int damageToDeal = stats.att - enemy.stats.def;

            if (damageToDeal > 0)
            {
                enemy.stats.hp -= damageToDeal;
            }
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

        public bool IsInRange(Point p)
        {
            return DistanceTo(p.X, p.Y) == Range;
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
            if(owner == player)
            {
                movesLeft = moveSpeed;
            }
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

        public bool IsDead => stats.hp <= 0;

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

        public int Range => 1;

        /// <summary>
        /// The Stats of this Unit. 
        /// </summary>
        /// <see cref="Units.Stats"/>
        public Stats Stats
        {
            get
            {
                return stats;
            }
            protected set
            {
                stats = value;
            }
        }
    }
}
