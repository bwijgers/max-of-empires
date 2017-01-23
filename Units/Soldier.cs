using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui;
using MaxOfEmpires.Files;
using MaxOfEmpires.GameObjects;
using System;

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

            // Load all specials of the Unit from config
            int specialsList = config.GetProperty<int>("specialties");

            Soldier prototype = new Soldier(config.GetProperty<string>("name"), 0, 0, new Player("none", "blue", Color.Black, 100), texName, moveSpeed, stats, range);
            prototype.Specials = specialsList;

            return prototype;
        }

        private bool animateDeath;
        private double animateDeathCounter;

        /// <summary>
        /// The name of this type of Soldier.
        /// </summary>
        private string name;

        /// <summary>
        /// The range in which this Unit can attack.
        /// </summary>
        private Range range;

        /// <summary>
        /// Specials. Don't change this. Used in bool properties.
        /// </summary>
        private int specials;

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
            animateDeath = false;
        }

        /// <summary>
        /// Creates a deep copy of this Unit.
        /// </summary>
        /// <param name="original">The original to make a copy of.</param>
        /// <param name="owner">The owner of the copy of this Unit.</param>
        public Soldier(Soldier original, Player owner) : this(new string(original.name.ToCharArray()), original.PositionInGrid.X, original.PositionInGrid.Y, owner, original.texName, original.moveSpeed, original.stats.Copy(), original.range.Copy())
        {
            specials = original.specials;
        }

        /// <summary>
        /// Attacks a Unit at the specified position. Assumes the enemy is in range.
        /// </summary>
        /// <param name="attackPos">The tile to attack.</param>
        public void Attack(Tile attackPos, bool retaliate)
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

            Soldier enemy = u as Soldier;
            OnSoldierStartAttack(enemy, retaliate);

            // Unit has attacked
            if (enemy.IsDead && Special_Assassin)
            {
                // Nothing is true, everything is permitted
                movesLeft = MoveSpeed;
                hasAttacked = false;
            }
            else
            {
                hasAttacked = true;
                HasMoved = !Special_IsRider;
            }

            // Don't do anything else if the enemy is dead. 
            if (enemy.IsDead)
                return;

            // Enemy can retaliate. Right?
            if (enemy.IsInRange(PositionInGrid))
            {
                // Okay, retaliate. 
                enemy.OnSoldierStartAttack(this, true);
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

            // Make sure the texture is loaded, or the game will crash.
            copy.LoadTexture();

            // Return the Unit copy
            return copy;
        }

        /// <summary>
        /// Deals damage to an enemy Unit, based on attack and defence. Calculates miss as well.
        /// </summary>
        /// <param name="enemy">The enemy to deal damage to.</param>
        private void DealDamage(Soldier enemy, bool retaliate)
        {
            // Check if we hit at all
            int enemyDodgeBonus = (enemy.Parent as Tile).DodgeBonus;
            if (enemy.owner == ((Parent as Tile).Parent as Grid).attackingPlayer)
            {
                enemyDodgeBonus += ((Parent as Tile).Parent as BattleGrid).attackingTile.DodgeBonus;
            }
            else
            {
                enemyDodgeBonus += ((Parent as Tile).Parent as BattleGrid).defendingTile.DodgeBonus;
            }
            int hitChance = stats.hit - (enemy.stats.dodge+ enemyDodgeBonus);
            if (hitChance < 100)
            {
                double randDouble = MaxOfEmpires.Random.NextDouble();
                if (randDouble * 100 > hitChance)
                {
                    return; // we missed :c
                }
            }

            // We hit :D Damage the enemy
            // Check to see if we might be able to deal MOAR damage :))))
            int attack = stats.att;

            // Are we strong against this kind of enemy?
            if (enemy.Special_IsRider && Special_HorseBuster)
            {
                attack += stats.att * 2;
            }

            // Are we good retaliators who retaliate? 
            if (retaliate && Special_Tank)
            {
                attack += stats.att;
            }

            // Are we fighting a tank and are we tankbusters?
            if (Special_TankBuster && enemy.Special_Tank)
            {
                attack += (int)(stats.att * 0.5F);
            }

            int damageToDeal = attack;
            if (!Special_MagicFighter)
            {
                int enemyDefenseBonus = (enemy.Parent as Tile).DefenseBonus;
                if (enemy.owner == ((Parent as Tile).Parent as Grid).attackingPlayer)
                {
                    enemyDefenseBonus += ((Parent as Tile).Parent as BattleGrid).attackingTile.DefenseBonus;
                }
                else
                {
                    enemyDefenseBonus += ((Parent as Tile).Parent as BattleGrid).defendingTile.DefenseBonus;
                }
                damageToDeal -= (enemy.stats.def + enemyDefenseBonus);
            }
            
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

        public void OnDeath()
        {
            owner.AddUnitLostToStats(id);
            animateDeath = true;
        }

        public void OnSoldierStartAttack(Soldier enemy, bool retaliate)
        {
            // Magic fighters can't retaliate D:
            if (Special_MagicFighter && retaliate)
                return;

            // TODO: Call animation code here

            // This should be called at the right time during the animation
            DealDamage(enemy, retaliate);
        }

        public override void TurnUpdate(uint turn, Player player, GameTime t)
        {
            base.TurnUpdate(turn, player, t);
            hasAttacked = false;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            if (IsDead)
            {
                animateDeathCounter += time.ElapsedGameTime.TotalSeconds / 2.0D;
                DrawColor = new Color(DrawColor, (float)(Math.Cos(animateDeathCounter / 2.0D * Math.PI)));
                if (DrawColor.A <= 10)
                {
                    ((GameWorld as Grid)[PositionInGrid] as Tile).SetUnit(null);
                }
            }
        }

        public override bool HasAction
        {
            get
            {
                return (!HasAttacked && !IsDead) || base.HasAction;
            }
        }

        /// <summary>
        /// Whether this Soldier has attacked this turn.
        /// </summary>
        public bool HasAttacked => hasAttacked;

        /// <summary>
        /// Whether this Soldier is dead.
        /// </summary>
        public bool IsDead => animateDeath || stats.hp <= 0;

        /// <summary>
        /// The name of this type of Soldier.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The range at which this Soldier can attack.
        /// </summary>
        public Range Range => range;

        /// <summary>
        /// Whether this Soldier is a rider.
        /// </summary>
        public bool Special_HorseBuster => (specials & 1) == 1;
        public bool Special_IsRider => (specials & 2) == 2;
        public bool Special_MagicFighter => (specials & 4) == 4;
        public bool Special_Tank => (specials & 8) == 8;
        public bool Special_TankBuster => (specials & 16) == 16;
        public bool Special_Assassin => (specials & 32) == 32;

        private int Specials
        {
            get
            {
                return specials;
            }
            set
            {
                specials = value;
            }
        }

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
