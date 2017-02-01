using System.Text;
﻿using MaxOfEmpires.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private int tier;
        private HitEffects h;

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
        public static Soldier LoadFromConfiguration(Configuration config, int tier)
        {
            // Load stats from config
            Stats stats = Stats.LoadFromConfiguration(config.GetPropertySection("stats."+tier));

            // Load range from config
            Range range = Range.LoadFromConfiguration(config.GetPropertySection("range."+tier));

            // Load movespeed from config
            int moveSpeed = config.GetProperty<int>("moveSpeed."+tier);

            // Load texture from config file
            string texName = config.GetProperty<string>("texture.name");
            //string texName = "swordsman";

            // Load all specials of the Unit from config
            int specialsList = config.GetProperty<int>("specialties");

            Soldier prototype = new Soldier(config.GetProperty<string>("name."+tier), 0, 0, new Player("none", "blue", Color.Black, 100), texName, moveSpeed, stats, range, tier);
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

        /// <summary>
        /// Float values used as a timer during the attack animation, an below that two bools used for timing the attack animation.
        /// </summary>

        public float attackAnimationTimer;
        public float attackAnimationFrames = 1f;

        /// <summary>
        /// The following vectors are used to create the attack animation and to determine which walking animation should be played during it.
        /// </summary>

        private Vector2 normalizedAttackDirection;
        private Vector2 walkUpDirection = new Vector2(0, -1);
        private Vector2 walkRightDirection = new Vector2(1, 0);
        private Vector2 walkDownDirection = new Vector2(0, 1);
        private Vector2 walkLeftDirection = new Vector2(-1, 0);
        private Vector2 walkDirectionZero = new Vector2(0, 0);

        public bool duringAttack = false, retaliating = false;
        private bool attacked = false;
        private bool healed = false;
        private Soldier attackTarget;
        
        private string texName;

        private Soldier(string name, int x, int y, Player owner, string resName, int moveSpeed, Stats stats, Range range, int tier) : base(x, y, owner)
        {
            // Set parameters
            this.name = name;
            this.range = range;
            Stats = stats;
            texName = resName;
            this.moveSpeed = moveSpeed;
            animateDeath = false;
            this.tier = tier;
        }

        /// <summary>
        /// Creates a deep copy of this Unit.
        /// </summary>
        /// <param name="original">The original to make a copy of.</param>
        /// <param name="owner">The owner of the copy of this Unit.</param>
        public Soldier(Soldier original, Player owner) : this(new string(original.name.ToCharArray()), original.PositionInGrid.X, original.PositionInGrid.Y, owner, original.texName, original.moveSpeed, original.stats.Copy(), original.range.Copy(), original.tier)
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

            // Calls a certain method, which sets the damage calculation in action
            OnSoldierStartAttack(enemy, retaliate);

            // Unit has attacked. Healer shouldn't lose their attack since they can't attack.
            if(!Special_Healer)
                hasAttacked = true;
        }

        //Like Attack, but for healers.
        public void Heal(Tile healPos)
        {
            //Make sure that only the healers can heal.
            if (!Special_Healer)
            {
                return;
            }

            if (!healPos.Occupied)
            {
                return; 
            }

            Unit u = healPos.Unit;

            if(!(u is Soldier))
            {
                return;
            }

            Soldier ally = u as Soldier;

            //Start the animation.
            h = new HitEffects(this.name);
            h.DeterminePosition(ally.PositionInGrid);
            (GameWorld as Grid).hitEffectList.Add(h);

            //Make sure the animation runs for the appropriate amount of time.
            attackAnimationTimer = 0;
            healed = true;

            HealDamage(ally);

            //Make sure the healer can only heal once.
            hasAttacked = true;
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
            int enemyDodgeBonus = (enemy.Parent as Tile).DodgeBonus * 2;
            if (enemy.owner == (GameWorld as Grid).attackingPlayer)
            {
                enemyDodgeBonus += (GameWorld as BattleGrid).attackingTile.DodgeBonus;
            }
            else
            {
                enemyDodgeBonus += (GameWorld as BattleGrid).defendingTile.DodgeBonus;
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

            // The damageFalloff function makes it so ranged attacks for non-magical ranged units decay in strenght when attacking an enemy far away. Is used in the damage calculation below.
            double damageFalloff = (((Math.Abs(DrawPosition.X - enemy.DrawPosition.X) / 36) + (Math.Abs(DrawPosition.Y - enemy.DrawPosition.Y) / 36)));
            double calculatedFalloff = Math.Pow(0.95, damageFalloff);
            

            int damageToDeal = attack;
            if (!Special_MagicFighter && !Special_Bowman)
            {
                int enemyDefenseBonus = (enemy.Parent as Tile).DefenseBonus * 2;
                if (enemy.owner == (GameWorld as Grid).attackingPlayer)
                {
                    enemyDefenseBonus += (GameWorld as BattleGrid).attackingTile.DefenseBonus;
                }
                else
                {
                    enemyDefenseBonus += (GameWorld as BattleGrid).defendingTile.DefenseBonus;
                }
                int baseDefense = (enemy.stats.def + enemyDefenseBonus);
                damageToDeal -= baseDefense;

            }
            else if (Special_Bowman)
            {
                int enemyDefenseBonus = (enemy.Parent as Tile).DefenseBonus * 2;
                if (enemy.owner == (GameWorld as Grid).attackingPlayer)
                {
                    enemyDefenseBonus += (GameWorld as BattleGrid).attackingTile.DefenseBonus;
                }
                else
                {
                    enemyDefenseBonus += (GameWorld as BattleGrid).defendingTile.DefenseBonus;
                }
                int baseDefense = (enemy.stats.def + enemyDefenseBonus);
                damageToDeal = ((int)(damageToDeal * calculatedFalloff) - baseDefense);
            }

            
             
            
            // If there is no damage to deal, don't actually *heal* the enemy Unit.
            if (damageToDeal > 0)
            {
                enemy.stats.hp -= damageToDeal;
            }
        }

        // Like DealDamage, but with an H.
        private void HealDamage(Soldier ally)
        {
            int missingHp = ally.stats.maxHp - ally.stats.hp;

            //Here we determine the amount that will be healed which is a third of the maxHp of the target being healed times the tier of the healer. We can divide by 3 because all maxHps are multiples of 6
            int heal = (ally.stats.maxHp / 3) *tier;

            // Dont heal past the maximum hp.
            if (missingHp > heal)
            {
                ally.stats.hp += heal;
            }
            else
            {
                ally.stats.hp = ally.stats.maxHp;
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
            AssetManager.Instance.PlaySound("Music/Soldierfade");
            //owner.AddUnitLostToStats(id);
            animateDeath = true;
        }

        public void OnSoldierStartAttack(Soldier enemy, bool retaliate)
        {
            // Healers can't fight :'C
            if (Special_Healer)
                return;
            // Magic fighters can't retaliate D:
            if (Special_MagicFighter && retaliate)
                return;

            // Start of animation code
            attackTarget = enemy;

            Vector2 tPos = enemy.DrawPosition;
            Vector2 cPos = DrawPosition;

            Vector2 attackDirection = tPos-cPos;
            Vector2.Normalize(ref attackDirection, out normalizedAttackDirection);

            if (normalizedAttackDirection == walkUpDirection)
                DrawingTexture.SelectedSprite = new Point(0, ANIMATION_WALK_UP);

            else if (normalizedAttackDirection == walkDownDirection)
                DrawingTexture.SelectedSprite = new Point(0, ANIMATION_WALK_DOWN);

            else if (normalizedAttackDirection == walkRightDirection || normalizedAttackDirection.X > walkDirectionZero.X)
                DrawingTexture.SelectedSprite = new Point(0, ANIMATION_WALK_RIGHT);

            else if (normalizedAttackDirection == walkLeftDirection || normalizedAttackDirection.X < walkDirectionZero.X)
                DrawingTexture.SelectedSprite = new Point(0, ANIMATION_WALK_LEFT);

            position = position + (normalizedAttackDirection * 10);

            h = new HitEffects(this.name);
            h.DeterminePosition(enemy.PositionInGrid);
            (GameWorld as Grid).hitEffectList.Add(h);

            attackAnimationTimer = 0f;
            attacked = true;
            duringAttack = true;
            retaliating = retaliate;
        }


        public void UpdateAttack()
        { // Written by: TheMez
            // This method starts whenever the 'attacked' bool is set to true.
            if (attackAnimationTimer > attackAnimationFrames)
            {
                position = position - (normalizedAttackDirection * 10);
                (GameWorld as Grid).hitEffectList.RemoveChild(h);
                DealDamage(attackTarget, retaliating);
                DrawingTexture.SelectedSprite = new Point(0, ANIMATION_IDLE);

                hasAttacked = true;
                HasMoved = !Special_IsRider;

                if (attackTarget.IsDead)
                {
                    (GameWorld as BattleGrid).OnKillSoldier(attackTarget);
                    
                    // Unit has attacked
                    if (Special_Assassin)
                    {
                        // Nothing is true, everything is permitted
                        movesLeft = MoveSpeed;
                        hasAttacked = false;
                    }
                }
                else if (!retaliating && attackTarget.IsInRange(PositionInGrid))
                {
                    attackTarget.OnSoldierStartAttack(this, true);
                }

                attacked = false;
                duringAttack = false;
                retaliating = false;
            }
        }

        // Like UpdateAttack but, for healers.
        public void UpdateHeal()
        {
            if (attackAnimationTimer > attackAnimationFrames)
            {
                (GameWorld as Grid).hitEffectList.RemoveChild(h);
                healed = false;
            }

        }

        public override void TurnUpdate(uint turn, Player player, GameTime t)
        {
            base.TurnUpdate(turn, player, t);
            hasAttacked = false;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            attackAnimationTimer += (float)time.ElapsedGameTime.TotalSeconds;

            if (attacked)
                UpdateAttack();
            if (healed)
                UpdateHeal();

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
        public bool Special_Healer => (specials & 64) == 64;
        public bool Special_Bowman => (specials & 128) == 128;

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
