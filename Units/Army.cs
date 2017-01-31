using Ebilkill.Gui.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

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
            retVal.unitsAndCounts["unit.swordsman.2"] = rand.Next(1, 2);
            retVal.unitsAndCounts["unit.archer.2"] = rand.Next(1, 3);
            retVal.unitsAndCounts["unit.mage.2"] = rand.Next(1, 3);
            retVal.unitsAndCounts["unit.horse.2"] = rand.Next(1, 3);
            retVal.unitsAndCounts["unit.heavy.2"] = rand.Next(1, 3);
            retVal.unitsAndCounts["unit.assassin.2"] = rand.Next(1, 3);

            // Return the newly generated Army
            return retVal;
        }

        public static Army LoadFromFile(BinaryReader reader, List<Player> players)
        {
            // Get information to recreate the Army
            Point positionInGrid = new Point(reader.ReadInt16(), reader.ReadInt16());
            string owner = reader.ReadString();

            // Create the Army from this information
            Army retVal = new Army(positionInGrid.X, positionInGrid.Y, players.Find(p => p.Name.Equals(owner)));

            // Update the Army's fields
            retVal.movesLeft = reader.ReadByte();
            retVal.id = reader.ReadString();
            retVal.TargetPosition = new Point(reader.ReadInt16(), reader.ReadInt16());

            // Get the actual army part of the Army
            byte soldierTypeCount = reader.ReadByte();
            for (int i = 0; i < soldierTypeCount; ++i)
            {
                retVal.unitsAndCounts[reader.ReadString()] = reader.ReadByte();
            }

            // Load the army sprite
            retVal.UpdateArmySprite();

            // I think we're done. Return the Army
            return retVal;
        }

        /// <summary>
        /// The Soldiers and the amount of each Soldier in this Army.
        /// </summary>
        private Dictionary<string, int> unitsAndCounts; // It's a UAC, guys :o

        private Dictionary<string, int> selectedUnits;

        private bool refreshInfo;

        /// <summary>
        /// Creates a new empty Army.
        /// </summary>
        /// <param name="x">The x-position on which this Army stands.</param>
        /// <param name="y">The y-position on which this Army stands.</param>
        /// <param name="owner">The player that is the owner of this Army.</param>
        public Army(int x, int y, Player owner) : base(x, y, owner)
        {
            unitsAndCounts = new Dictionary<string, int>();
            selectedUnits = new Dictionary<string, int>();
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
                selectedUnits[unitName] = currentSoldiers[unitName];
            }
        }

        public void AddSelected(string s)
        {
            if (selectedUnits[s] < unitsAndCounts[s])
                selectedUnits[s]++;
        }

        public void AddSoldier(Soldier s)
        {
            if (!unitsAndCounts.ContainsKey(s.Name))
            {
                unitsAndCounts[s.Name] = 0;
                selectedUnits[s.Name] = 0;
            }
            ++unitsAndCounts[s.Name];
            ++selectedUnits[s.Name];

            UpdateArmySprite();
        }

        private bool AreAllUnitsSelected()
        {
            foreach (string name in unitsAndCounts.Keys)
            {
                if (selectedUnits[name] != unitsAndCounts[name])
                {
                    return false;
                }
            }
            return true;

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

        public int GetTotalUnitCount()
        {
            int amount = 0;
            foreach(int i in unitsAndCounts.Values)
            {
                amount += i;
            }
            return amount;
        }

        public void LowerSelected(string s)
        {
            if (selectedUnits[s] > 0)
                selectedUnits[s]--;
        }

        /// <summary>
        /// Merges an army into this army. 
        /// </summary>
        /// <param name="other">The army to merge into this army.</param>
        public bool MergeArmy(Army other)
        {
            if(GetTotalUnitCount()+other.GetTotalUnitCount() > 20)
            {
                return false;
            }
            // Add all units of the other army to this army.
            foreach (string s in other.unitsAndCounts.Keys)
            {
                if (!unitsAndCounts.ContainsKey(s))
                {
                    unitsAndCounts[s] = 0;
                    selectedUnits[s] = 0;
                }
                unitsAndCounts[s] += other.unitsAndCounts[s];
                selectedUnits[s] += other.unitsAndCounts[s];
            }

            // Armies can move as far as the slowest Soldier, eh? 
            MoveSpeed = Math.Min(moveSpeed, other.moveSpeed);
            MovesLeft = Math.Min(movesLeft, other.movesLeft);

            // Update the sprite
            UpdateArmySprite();
            return true;
        }

        public void SelectAllUnits()
        {
            foreach (string soldierType in unitsAndCounts.Keys)
            {
                selectedUnits[soldierType] = unitsAndCounts[soldierType];
            }
        }

        public Army SplitArmy(Dictionary<string, int> unitsAndCounts)
        {
            // TODO: Test and call this
            // Check if we have enough of every Soldier type
            foreach (string s in unitsAndCounts.Keys)
            {
                if (!this.unitsAndCounts.ContainsKey(s) || this.unitsAndCounts[s] < unitsAndCounts[s])
                {
                    // Return no army if we don't
                    return null;
                }
            }

            // Return the requested army if we do, and subtract the units from this army
            Army retVal = new Army(PositionInGrid.X, PositionInGrid.Y, owner);
            foreach (string s in unitsAndCounts.Keys)
            {
                if (!retVal.unitsAndCounts.ContainsKey(s))
                {
                    retVal.unitsAndCounts[s] = 0;
                    retVal.selectedUnits[s] = 0;
                }
                retVal.unitsAndCounts[s] += unitsAndCounts[s];
                retVal.selectedUnits[s] += unitsAndCounts[s];
                this.unitsAndCounts[s] -= unitsAndCounts[s];
            }

            retVal.MovesLeft = MovesLeft;
            retVal.MoveSpeed = MoveSpeed;
            retVal.Parent = Parent;

            UpdateArmySprite();
            retVal.UpdateArmySprite();

            return retVal;
        }

        public override void TurnUpdate(uint turn, Player player, GameTime t)
        {
            SelectAllUnits();
            this.moveSpeed = GetSlowestUnit().MoveSpeed;
            base.TurnUpdate(turn, player,t);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            // If there are no more Soldiers in this stack, it will stop existing
            if (unitsAndCounts.Keys.Count == 0)
            {
                CurrentTile.SetUnit(null);
                return;
            }
        }

        private void UpdateArmySprite()
        {
            // Set the drawing texture to the Unit that is most prevalent in this stack
            int maxUnits = 0;
            Dictionary<string, int> newUAC = new Dictionary<string, int>();
            Dictionary<string, int> newSelected = new Dictionary<string, int>();
            foreach (string soldierType in unitsAndCounts.Keys)
            {
                if (unitsAndCounts[soldierType] > maxUnits)
                {
                    Soldier soldier = SoldierRegistry.GetSoldier(soldierType, owner);
                    DrawingTexture = soldier.DrawingTexture;
                    maxUnits = unitsAndCounts[soldierType];
                }

                if (unitsAndCounts[soldierType] != 0)
                {
                    newUAC[soldierType] = unitsAndCounts[soldierType];
                    newSelected[soldierType] = newUAC[soldierType];
                }
            }

            unitsAndCounts = newUAC;
            selectedUnits = newSelected;

            refreshInfo = true;
        }

        public override void WriteToFile(BinaryWriter writer)
        {
            base.WriteToFile(writer);

            writer.Write((byte)(unitsAndCounts.Count & 255));
            foreach (string soldierName in unitsAndCounts.Keys)
            {
                writer.Write(soldierName);
                writer.Write((byte) (unitsAndCounts[soldierName] & 255));
            }
        }

        /// <summary>
        /// The Tile this Army currently stands on.
        /// </summary>
        private Tile CurrentTile => (GameWorld as Grid)[PositionInGrid] as Tile;

        public bool AllUnitsSelected => AreAllUnitsSelected();

        /// <summary>
        /// The Soldiers and the amount of each Soldier in this Army.
        /// </summary>
        public Dictionary<string, int> UnitsAndCounts => unitsAndCounts;

        public bool RefreshInfo
        {
            get
            {
                return refreshInfo;
            }
            set
            {
                refreshInfo = value;
            }
        }

        public Dictionary<string, int> SelectedUnits => selectedUnits;
    }
}
