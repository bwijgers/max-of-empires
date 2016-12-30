using MaxOfEmpires.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui;
using MaxOfEmpires.Files;

namespace MaxOfEmpires.Units
{
    partial class Unit : GameObjectDrawable
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

        private string texName;

        /// <summary>
        /// The x and y coords of this Unit. Used for drawing and moving.
        /// </summary>
        private int x, y;

        public Unit(int x, int y, bool owner, string resName, int moveSpeed, Stats stats)
        {
            // Set parameters
            this.x = x;
            this.y = y;
            this.owner = owner;
            Stats = stats;
            this.texName = resName;
            this.moveSpeed = moveSpeed;

            // Set others
            target = new Point(x, y);
        }

        public Unit(Unit original, bool owner) : this(original.x, original.y, owner, original.texName, original.moveSpeed, original.stats.Copy())
        {
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
            if (enemy.IsInRange(PositionInGrid))
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
            // Draw the Unit based on whether it still has moves left; gray if there are no moves left.
            DrawColor = HasMoved ? Color.Gray : Color.White;
            base.Draw(time, s);

            // Draw a health bar
            DrawHealthBar(s);
        }

        /// <summary>
        /// Draws a healthbar for this Unit.
        /// </summary>
        /// <param name="s">The spritebatch to draw with.</param>
        private void DrawHealthBar(SpriteBatch s)
        {
            DrawingHelper.Instance.DrawRectangle(s, GetRectangleHealthBar(true), Color.Red);
            DrawingHelper.Instance.DrawRectangle(s, GetRectangleHealthBar(false), Color.Blue);
        }

        /// <summary>
        /// A rectangle for drawing the healthbar. Returns the width based on whether it's the foreground or background of the bar.
        /// </summary>
        /// <param name="background">Whether this is the foreground or the background layer of the healthbar.</param>
        /// <returns>The Rectangle on which this layer of the healthbar should be drawn.</returns>
        private Rectangle GetRectangleHealthBar(bool background)
        {
            // Base coords
            int x = (int)DrawPosition.X;
            int y = (int)DrawPosition.Y + 26;

            // Base sizes
            int height = 6;
            int width = 32;

            // Makes the foreground smaller based on hp/maxhp
            if (!background)
            {
                double widthMult = stats.hp;
                widthMult /= stats.maxHp;
                width = (int)(width * widthMult);
            }

            // Returns the calculated Rectangle
            return new Rectangle(x, y, width, height);
        }

        public bool IsInRange(Point p)
        {
            return DistanceTo(p.X, p.Y) == Range;
        }

        public static Unit LoadFromConfiguration(Configuration config)
        {
            // Load stats from config
            Stats stats = Stats.LoadFromConfiguration(config.GetPropertySection("stats"));

            // Load movespeed from config
            int moveSpeed = config.GetProperty<int>("moveSpeed");

            // Load texture from config file
            string texName = config.GetProperty<string>("texture.name");
            return new Unit(0, 0, false, texName, moveSpeed, stats);
        }

        public void LoadTexture()
        {
            // Get the texture based on the player (blue for p1, red for p2)
            StringBuilder texName = new StringBuilder();
            texName.Append(@"FE-Sprites\").Append(this.texName).Append('_');
            texName.Append(owner ? "blue" : "red");

            // Load the Unit's texture based on the name supplied and the player controlling the unit.
            DrawingTexture = AssetManager.Instance.getAsset<Texture2D>(texName.ToString());
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
            int distance = ShortestPath(new Point(x, y)).cost;

            // Check if we can move to this position before actually just moving there. CanMoveTo decrements MovesLeft as well, if it is possible to move to the position.
            if (distance <= movesLeft)
            {
                this.x = x;
                this.y = y;
                movesLeft -= distance;
                return true;
            }
            return false;
        }

        public Unit Copy(bool owner)
        {
            // Create a new Unit instance
            Unit copy = new Unit(this, owner);

            // Populate the Unit's values
            copy.moveSpeed = moveSpeed;
            copy.stats = stats.Copy();

            // Load the texture
            copy.LoadTexture();

            // Return the Unit copy
            return copy;
        }

        public override void TurnUpdate(uint turn, bool player)
        {
            if (owner == player)
            {
                movesLeft = moveSpeed;
            }
            hasAttacked = false;
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
        /// Whether this Unit is dead.
        /// </summary>
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
        public bool Owner
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
        /// The range at which this Unit can attack.
        /// </summary>
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
