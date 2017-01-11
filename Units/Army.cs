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
        private Dictionary<string, int> unitsAndCounts; // It's a UAC, guys :o

        /// <summary>
        /// Creates a new empty Army.
        /// </summary>
        /// <param name="x">The x-position on which this Army stands.</param>
        /// <param name="y">The y-position on which this Army stands.</param>
        /// <param name="owner">The player that is the owner of this Army.</param>
        public Army(int x, int y, bool owner) : base(x, y, owner)
        {
            unitsAndCounts = new Dictionary<string, int>();
        }

        /// <summary>
        /// Creates a new populated army.
        /// </summary>
        /// <param name="x">The x-position on which this Army stands.</param>
        /// <param name="y">The y-position on which this Army stands.</param>
        /// <param name="owner">The player that is the owner of this Army.</param>
        /// <param name="currentSoldiers">The Soldiers to add to this Army.</param>
        public Army(int x, int y, bool owner, Dictionary<string, int> currentSoldiers) : this(x, y, owner)
        {
            foreach (string unitName in currentSoldiers.Keys)
            {
                unitsAndCounts[unitName] = currentSoldiers[unitName];
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
            retVal.unitsAndCounts["unit.swordsman"] = rand.Next(1, 2);
            retVal.unitsAndCounts["unit.archer"] = rand.Next(1, 3);

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
            ICollection<string> unitsInStack = unitsAndCounts.Keys;

            // Get the movespeed of the slowest one.
            int moveSpeed = 99999;
            Soldier slowest = null;
            foreach (string unitName in unitsInStack)
            {
                Soldier u = SoldierRegistry.GetSoldier(unitName, owner);
                moveSpeed = Math.Min(moveSpeed, u.MoveSpeed);
                slowest = u.Copy(owner);
            }

            /* TODO: remove this
            // Set the slowest one to this position in this grid
            slowest.Parent = Parent;
            slowest.PositionInGrid = PositionInGrid;
            slowest.MovesLeft = movesLeft;
            slowest.TargetPosition = TargetPosition;
            */
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
            foreach (string unitName in unitsAndCounts.Keys)
            {
                if (unitsAndCounts[unitName] > maxUnits)
                {
                    Soldier u = SoldierRegistry.GetSoldier(unitName, owner);
                    DrawingTexture = u.DrawingTexture;
                    maxUnits = unitsAndCounts[unitName];
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
        public Dictionary<string, int> UnitsAndCounts => unitsAndCounts;
    }
}
