﻿using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui;
using MaxOfEmpires.Files;
using MaxOfEmpires.GameObjects;

namespace MaxOfEmpires.Units
{
    class Soldier : Unit
    {
        private const int ANIMATION_IDLE = 0;
        private const int ANIMATION_WALK_UP = 1;
        private const int ANIMATION_WALK_RIGHT = 2;
        private const int ANIMATION_WALK_DOWN = 3;
        private const int ANIMATION_WALK_LEFT = 4;

        /// <summary>
        /// Loads a Unit from a configuration. 
        /// </summary>
        /// Note that a Unit requires these keys:
        ///   - name (a string)
        ///   - stats (a Stats object. <see cref="Units.Stats.LoadFromConfiguration(Configuration)"/>
        ///   - range (a Range object. <see cref="Units.Range.LoadFromConfiguration(Configuration)"/> 
        ///   - texture.name (a string)
        /// <param name="config">The configuration file/subsection to load from.</param>
        /// <returns>A Unit as loaded from the configuration.</returns>
        public static Soldier LoadFromConfiguration(Configuration config)
        {
            // Load stats from config
            Stats stats = Stats.LoadFromConfiguration(config.GetPropertySection("stats"));

            // Load range from config
            Range range = Range.LoadFromConfiguration(config.GetPropertySection("range"));

            // Load movespeed from config
            int moveSpeed = config.GetProperty<int>("moveSpeed");

            // Load texture from config file
            string texName = config.GetProperty<string>("texture.name");
            //string texName = "swordsman";
            return new Soldier(config.GetProperty<string>("name"), 0, 0, new Player("none", "blue", 100), texName, moveSpeed, stats, range);
        }

        /// <summary>
        /// Loads a Unit from a configuration. 
        /// </summary>
        /// Note that a Unit requires these keys:
        ///   - name (a string)
        ///   - stats (a Stats object. <see cref="Units.Stats.LoadFromConfiguration(Configuration)"/>
        ///   - range (a Range object. <see cref="Units.Range.LoadFromConfiguration(Configuration)"/> 
        ///   - texture.name (a string)
        /// <param name="config">The configuration file/subsection to load from.</param>
        /// <returns>A Unit as loaded from the configuration.</returns>
        public static Soldier LoadFromConfiguration(Configuration config)
        {
            // Load stats from config
            Stats stats = Stats.LoadFromConfiguration(config.GetPropertySection("stats"));

            // Load range from config
            Range range = Range.LoadFromConfiguration(config.GetPropertySection("range"));

            // Load movespeed from config
            int moveSpeed = config.GetProperty<int>("moveSpeed");

            // Load texture from config file
            string texName = config.GetProperty<string>("texture.name");
            return new Soldier(config.GetProperty<string>("name"), 0, 0, false, texName, moveSpeed, stats, range);
        }

        /// <summary>
        /// The name of this type of Soldier.
        /// </summary>
        private string name;

        /// <summary>
        /// The range in which this Unit can attack.
        /// </summary>
        private Range range;

        /// <summary>
        /// This Unit's stats. 
        /// </summary>
        /// <see cref="Units.Stats"/>
        private Stats stats;

        private string texName;

        private Soldier(string name, int x, int y, Player owner, string resName, int moveSpeed, Stats stats, Range range) : base(x, y, owner)
        {
            // Set parameters
            this.name = name;
            this.range = range;
            Stats = stats;
            texName = resName;
            this.moveSpeed = moveSpeed;
        }

        /// <summary>
        /// Creates a deep copy of this Unit.
        /// </summary>
        /// <param name="original">The original to make a copy of.</param>
        /// <param name="owner">The owner of the copy of this Unit.</param>
        public Soldier(Soldier original, Player owner) : this(new string(original.name.ToCharArray()), original.PositionInGrid.X, original.PositionInGrid.Y, owner, original.texName, original.moveSpeed, original.stats.Copy(), original.range.Copy())
        {
            // Make sure the texture is loaded, or the game will crash.
            LoadTexture();
        }

        /// <summary>
        /// Attacks a Unit at the specified position. Assumes the enemy is in range.
        /// </summary>
        /// <param name="attackPos">The tile to attack.</param>
        public void Attack(Tile attackPos)
        {
            // Check if the attacking tile actually has a Unit to attack. 
            if (!attackPos.Occupied)
            {
                return; // This should *never* happen. 
            }

            // Actually damage the enemy unit.
            Unit u = attackPos.Unit;
            if(!(u is Soldier))
            {
                return; // SHOULD NEVER HAPPEN
            }

            // Unit has attacked
            hasAttacked = true;

            Soldier enemy = u as Soldier;
            OnSoldierStartAttack(enemy);

            // Don't do anything else if the enemy is dead. 
            if (enemy.IsDead)
                return;

            // Enemy can retaliate. Right?
            if (enemy.IsInRange(PositionInGrid))
            {
                // Okay, retaliate. 
                enemy.OnSoldierStartAttack(this);
            }
        }

        /// <summary>
        /// Copy this Unit and set an owner.
        /// </summary>
        /// <param name="owner">The owner of the Copy.</param>
        /// <returns>A copy of the Unit.</returns>
        public Soldier Copy(Player owner)
        {
            // Create a new Unit instance
            Soldier copy = new Soldier(this, owner);

            // Return the Unit copy
            return copy;
        }

        /// <summary>
        /// Deals damage to an enemy Unit, based on attack and defence. Calculates miss as well.
        /// </summary>
        /// <param name="enemy">The enemy to deal damage to.</param>
        private void DealDamage(Soldier enemy)
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

            // If there is no damage to deal, don't actually *heal* the enemy Unit.
            if (damageToDeal > 0)
            {
                enemy.stats.hp -= damageToDeal;
            }
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
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

        /// <summary>
        /// Checks whether the specified position is in the Range of this Unit.
        /// </summary>
        /// <param name="p">The position to check.</param>
        /// <returns>True if the position is in range, false otherwise.</returns>
        public bool IsInRange(Point p)
        {
            return range.InRange(DistanceTo(p.X, p.Y));
        }

        /// <summary>
        /// Whether this Soldier is of the same type as another. For instance, whether both are Archers or both are Snipers.
        /// </summary>
        /// <param name="other">The other Soldier to see whether these are of equal type.</param>
        /// <returns>True when the Soldiers are of equal type, false otherwise.</returns>
        public bool IsSameType(Soldier other)
        {
            return other.name.Equals(name);
        }

        public void LoadTexture()
        {
            // Get the texture based on the player (blue for p1, red for p2)
            StringBuilder texName = new StringBuilder();
            texName.Append(@"FE-Sprites\Units\").Append(this.texName); // Correct again
            texName.Append(owner.ColorName).Append("@4x5");

            // Load the Unit's texture based on the name supplied and the player controlling the unit.
            DrawingTexture = AssetManager.Instance.getAsset<Spritesheet>(texName.ToString());

        }

        public void OnSoldierStartAttack(Soldier enemy)
        {
            // TODO: Call animation code here

            // This should be called at the right time during the animation
            DealDamage(enemy);
        }

        public override void TurnUpdate(uint turn, Player player)
        {
            base.TurnUpdate(turn, player);
            hasAttacked = false;
        }

        public override bool HasAction
        {
            get
            {
                return base.HasAction || !HasAttacked;
            }
        }

        /// <summary>
        /// Whether this Unit has attacked this turn.
        /// </summary>
        public bool HasAttacked => hasAttacked;

        /// <summary>
        /// Whether this Unit is dead.
        /// </summary>
        public bool IsDead => stats.hp <= 0;

        /// <summary>
        /// The name of this type of Soldier.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The range at which this Soldier can attack.
        /// </summary>
        public Range Range => range;

        /// <summary>
        /// The Stats of this Soldier. 
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
