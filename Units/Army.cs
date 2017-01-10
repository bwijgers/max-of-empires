using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Units
{
    class Army : Unit
    {
        /// <summary>
        /// The Soldiers and the amount of each Soldier in this Army.
        /// </summary>
        private Dictionary<Soldier, int> unitsAndCounts; // It's a UAC, guys :o

        /// <summary>
        /// Creates a new empty Army.
        /// </summary>
        /// <param name="x">The x-position on which this Army stands.</param>
        /// <param name="y">The y-position on which this Army stands.</param>
        /// <param name="owner">The player that is the owner of this Army.</param>
        public Army(int x, int y, bool owner) : base(x, y, owner)
        {
            unitsAndCounts = new Dictionary<Soldier, int>();
        }

        /// <summary>
        /// Creates a new populated army.
        /// </summary>
        /// <param name="x">The x-position on which this Army stands.</param>
        /// <param name="y">The y-position on which this Army stands.</param>
        /// <param name="owner">The player that is the owner of this Army.</param>
        /// <param name="currentSoldiers">The Soldiers to add to this Army.</param>
        public Army(int x, int y, bool owner, Dictionary<Soldier, int> currentSoldiers) : this(x, y, owner)
        {
            foreach (Soldier u in currentSoldiers.Keys)
            {
                unitsAndCounts[u] = currentSoldiers[u];
            }
        }

        /// <summary>
        /// Creates a new Army with some Soldiers in it. 
        /// </summary>
        /// <param name="owner">The player that owns this Army.</param>
        /// <returns>The newly generated Army.</returns>
        public static Army GenerateArmy(bool owner)
        {
            // Create a new Army
            Army retVal = new Army(0, 0, owner);

            // Get the rng
            Random rand = MaxOfEmpires.Random;

            // Get the amount of Swordsmen and Archers we should have in this army
            retVal.unitsAndCounts[SoldierRegistry.GetSoldier("swordsman", owner)] = rand.Next(30, 45);
            retVal.unitsAndCounts[SoldierRegistry.GetSoldier("archer", owner)] = rand.Next(15, 30);

            // Return the newly generated Army
            return retVal;
        }

        /// <summary>
        /// Gets the Soldier in this Army with the lowest movement speed. Used to update moves left at the start of a turn.
        /// </summary>
        /// <returns>A copy of the Soldier with the lowest movement speed.</returns>
        private Soldier GetSlowestUnit()
        {
            // If this army is invalid, don't do shit. 
            if (unitsAndCounts == null || unitsAndCounts.Keys.Count == 0)
                return null; // Invalid army

            // Get all units
            ICollection<Soldier> unitsInStack = unitsAndCounts.Keys;

            // Get the movespeed of the slowest one.
            int moveSpeed = 99999;
            Soldier slowest = null;
            foreach (Soldier u in unitsInStack)
            {
                moveSpeed = Math.Min(moveSpeed, u.MoveSpeed);
                slowest = u.Copy(owner);
            }

            // Set the slowest one to this position in this grid
            slowest.Parent = Parent;
            slowest.PositionInGrid = PositionInGrid;
            slowest.MovesLeft = movesLeft;
            slowest.TargetPosition = TargetPosition;

            // Return the unit that was found slowest
            return slowest;
        }

        public override void TurnUpdate(uint turn, bool player)
        {
            this.moveSpeed = GetSlowestUnit().MoveSpeed;
            base.TurnUpdate(turn, player);
        }

        public override void Update(GameTime time)
        {
            // If there are no more Soldiers in this stack, it will stop existing
            if (unitsAndCounts.Keys.Count == 0)
            {
                CurrentTile.SetUnit(null);
                return;
            }

            // Set the drawing texture to the Unit that is most prevalent in this stack
            int maxUnits = 0;
            foreach (Soldier u in unitsAndCounts.Keys)
            {
                if (unitsAndCounts[u] > maxUnits)
                {
                    DrawingTexture = u.DrawingTexture;
                    maxUnits = unitsAndCounts[u];
                }
            }
        }

        /// <summary>
        /// The Tile this Army currently stands on.
        /// </summary>
        private Tile CurrentTile => ((GameWorld as Grid)[PositionInGrid] as Tile);

        /// <summary>
        /// The Soldiers and the amount of each Soldier in this Army.
        /// </summary>
        public Dictionary<Soldier, int> UnitsAndCounts => unitsAndCounts;
    }
}
