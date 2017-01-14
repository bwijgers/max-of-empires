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
        /// Creates a new Army with some Soldiers in it. 
        /// </summary>
        /// <param name="owner">The player that owns this Army.</param>
        /// <returns>The newly generated Army.</returns>
        public static Army GenerateArmy(Player owner)
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
        /// The Soldiers and the amount of each Soldier in this Army.
        /// </summary>
        private Dictionary<string, int> unitsAndCounts; // It's a UAC, guys :o

        /// <summary>
        /// Creates a new empty Army.
        /// </summary>
        /// <param name="x">The x-position on which this Army stands.</param>
        /// <param name="y">The y-position on which this Army stands.</param>
        /// <param name="owner">The player that is the owner of this Army.</param>
        public Army(int x, int y, Player owner) : base(x, y, owner)
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
        public Army(int x, int y, Player owner, Dictionary<string, int> currentSoldiers) : this(x, y, owner)
        {
            foreach (string unitName in currentSoldiers.Keys)
            {
                unitsAndCounts[unitName] = currentSoldiers[unitName];
            }
        }

        public void AddSoldier(Soldier s)
        {
            if (unitsAndCounts.ContainsKey(s.Name))
            {
                ++unitsAndCounts[s.Name];
                return;
            }
            unitsAndCounts[s.Name] = 1;
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
            foreach (string soldierType in unitsInStack)
            {
                Soldier soldier = SoldierRegistry.GetSoldier(soldierType, owner);
                moveSpeed = Math.Min(moveSpeed, soldier.MoveSpeed);
                if (moveSpeed == soldier.MoveSpeed)
                    slowest = soldier.Copy(owner);
            }

            // Return the unit that was found slowest
            return slowest;
        }

        public override void TurnUpdate(uint turn, Player player)
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
            foreach (string soldierType in unitsAndCounts.Keys)
            {
                if (unitsAndCounts[soldierType] > maxUnits)
                {
                    Soldier soldier = SoldierRegistry.GetSoldier(soldierType, owner);
                    DrawingTexture = soldier.DrawingTexture;
                    maxUnits = unitsAndCounts[soldierType];
                }
            }
        }

        /// <summary>
        /// The Tile this Army currently stands on.
        /// </summary>
        private Tile CurrentTile => (GameWorld as Grid)[PositionInGrid] as Tile;

        /// <summary>
        /// The Soldiers and the amount of each Soldier in this Army.
        /// </summary>
        public Dictionary<string, int> UnitsAndCounts => unitsAndCounts;
    }
}
