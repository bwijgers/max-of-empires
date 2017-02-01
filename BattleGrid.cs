using MaxOfEmpires.GameStates;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MaxOfEmpires
{
    class BattleGrid : Grid
    {
        public Tile attackingTile;
        public Tile defendingTile;


        public BattleGrid(int width, int height, List<Player> players, string id = "") : base(width, height, players, id)
        {
        }

        /// <summary>
        /// Checks whether a Unit can attack a Unit at the specified tile, and attacks it if it's possible.
        /// </summary>
        /// <param name="newPos">The position for the Unit to attack.</param>
        /// <param name="unit">The Unit which attacks.</param>
        /// <returns>True if the Unit attacked, false otherwise.</returns>
        public bool CheckAttackSoldier(Point tileToAttack, Soldier attackingUnit)
        {
            // Cannot attack more than once a turn. 
            if (attackingUnit.HasAttacked)
                return false;

            Tile toAttack = this[tileToAttack] as Tile;

            // Make sure the attack square is occupied by an enemy unit
            if (!toAttack.Occupied || toAttack.Unit.Owner == attackingUnit.Owner)
            {
                return false; // nothing to attack
            }

            // Make sure the attack square is in range of the attacking unit
            if (!attackingUnit.IsInRange(tileToAttack))
            {
                return false; // Enemy not in range
            }

            // We can actually attack this? Nice :D
            attackingUnit.Attack(toAttack, false);

            // After a battle, check if there are dead Units, and remove these if they are dead
            Soldier defender = toAttack.Unit as Soldier;

            return true;
        }

        //LikeCheckAttackSoldier, but for healers.
        public bool CheckHealHealer(Point tileToHeal, Soldier healingUnit)
        {
            if (healingUnit.HasAttacked)
                return false;

            Tile toHeal = this[tileToHeal] as Tile;

            if (!toHeal.Occupied || toHeal.Unit.Owner != healingUnit.Owner)
                return false;

            if (!healingUnit.IsInRange(tileToHeal))
                return false;

            healingUnit.Heal(toHeal);

            return true;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            base.Draw(time, s);
        }

        /// <summary>
        /// Called when a Soldier dies in battle.
        /// </summary>
        /// <param name="deadSoldier">The Soldier that died.</param>
        public void OnKillSoldier(Soldier deadSoldier)
        {
            // Remove this Soldier as it is dead
            deadSoldier.OnDeath();

            // If there are no Soldiers left on this side, the enemy won D:
            bool foundAlly = false;

            // Check all tiles
            ForEach(obj => {
                Tile t = obj as Tile;

                // If there is an ally, update this condition
                if (t.Occupied && t.Unit.Owner == deadSoldier.Owner && !(t.Unit as Soldier).IsDead)
                {
                    foundAlly = true;
                }
            });

            // If we found no ally, the enemy won.
            if (!foundAlly)
            {
                OnPlayerWinBattle(deadSoldier.Owner == attackingPlayer ? defendingPlayer : attackingPlayer);
            }
        }

        /// <summary>
        /// Executed when the player left-clicks on the grid.
        /// </summary>
        /// <param name="helper">The InputHelper used for mouse input.</param>
        public override void OnLeftClick(InputHelper helper)
        {
            bool animationBusy = false;
            ForEach(obj => {
                Tile t = obj as Tile;

                if (t.Occupied && (t.Unit as Soldier).duringAttack)
                {
                    animationBusy = true;
                    return;
                }
            });
            if (animationBusy)
            {
                return;
            }

            // Get the current Tile under the mouse
            Tile clickedTile = GetTileUnderMouse(helper, true);

            // Do nothing if there is no clicked tile.
            if (clickedTile == null)
                return;

            // If the player had a tile selected and it contains a Unit...
            if (SelectedTile != null && SelectedTile.Occupied)
            {
                // ... move the Unit there, if the square is not occupied and the unit is capable, then unselect the tile.
                SelectedTile.Unit.TargetPosition = clickedTile.PositionInGrid;
                Point movePos = Pathfinding.MoveTowardsTarget(SelectedTile.Unit);

                if (CheckMoveUnit(movePos, SelectedTile.Unit) || CheckAttackSoldier(clickedTile.PositionInGrid, (Soldier)SelectedTile.Unit) || CheckHealHealer(clickedTile.PositionInGrid, (Soldier)SelectedTile.Unit)|| movePos.Equals(SelectedTile.Unit.PositionInGrid))
                {
                    SelectTile(InvalidTile);
                    return;
                }
            }

            // Check if the player clicked a tile with a Unit on it, and select it if it's there. 
            else if (clickedTile.Occupied && clickedTile.Unit.Owner == CurrentPlayer && clickedTile.Unit.HasAction)
            {
                // If the Unit can walk, show where it is allowed to walk. 
                if (!clickedTile.Unit.HasMoved)
                {
                    walkablePositions = Pathfinding.ReachableTiles(clickedTile.Unit,Width,Height);
                    SetUnitWalkingOverlay(walkablePositions);
                }

                // This unit can be selected. Show the player it is selected too
                SelectTile(clickedTile.PositionInGrid);

                // Add an overlay for enemy units that can be attacked
                if (!(clickedTile.Unit as Soldier).HasAttacked)
                {
                    SetUnitAttackingOverlay((Soldier)clickedTile.Unit);
                }
            }
        }

        /// <summary>
        /// Called when a player wins a battle (because all enemies died).
        /// </summary>
        /// <param name="winningPlayer">The player that won the battle.</param>
        private void OnPlayerWinBattle(Player winningPlayer)
        {
            // Unset attacker and defender
            attackingPlayer = null;
            defendingPlayer = null;

            // Find what's left of our army
            Army remainingArmy = new Army(0, 0, winningPlayer);
            ForEach(obj => {
                Tile t = obj as Tile;

                // See if there's a Soldier belonging to the winning player here
                if (t.Occupied && t.Unit.Owner == winningPlayer)
                {
                    remainingArmy.AddSoldier(t.Unit as Soldier);
                }
            });

            // Tell economy grid what's left and go back to economy grid
            GameStateManager.OnPlayerWinBattle(remainingArmy);
        }

        /// <summary>
        /// Populates the field with the armies of both attacker and defender.
        /// </summary>
        /// <param name="attacker">The attacking Army.</param>
        /// <param name="defender">The defending Army.</param>
        public void PopulateField(Army attacker, Army defender)
        {
            // Initialize which players are the attacker and the defender
            attackingPlayer = attacker.Owner;
            defendingPlayer = defender.Owner;

            // Initialize the defender's field
            int currentX = 0;
            int currentY = 0;

            // Iterate over every type of Soldier in the defending Army
            foreach (string s in defender.UnitsAndCounts.Keys)
            {
                // Get the amount of this kind of Soldier
                int soldierCount = defender.UnitsAndCounts[s];

                // Place them in a position based on how many soldiers we have passed so far.
                for (; soldierCount > 0; --soldierCount)
                {
                    //only places soldier if terrain is passable
                    Soldier soldier = SoldierRegistry.GetSoldier(s, defender.Owner);
                    while (!(this[currentX, currentY] as Tile).Passable(soldier))
                    {
                        ++currentX;
                        if (currentX >= Width)
                        {
                            currentX = 0;
                            ++currentY;
                        }
                    }

                    (this[currentX, currentY] as Tile).SetUnit(soldier);
                    ++currentX;

                    // Make sure we don't create a line of soldiers longer than the field.
                    if (currentX >= Width)
                    {
                        currentX = 0;
                        ++currentY;
                    }
                }
            }

            // Do the same for the attacking Army, except start bottom right and go left->up for each Soldier.
            currentX = Width - 1;
            currentY = Height - 1;

            foreach (string s in attacker.UnitsAndCounts.Keys)
            {
                int soldierCount = attacker.UnitsAndCounts[s];

                for (; soldierCount > 0; --soldierCount)
                {
                    //only places soldier if terrain is passable
                    Soldier soldier = SoldierRegistry.GetSoldier(s, attacker.Owner);
                    while (!(this[currentX, currentY] as Tile).Passable(soldier))
                    {
                        --currentX;
                        if (currentX < 0)
                        {
                            currentX = Width - 1;
                            --currentY;
                        }
                    }
                    (this[currentX, currentY] as Tile).SetUnit(soldier);
                    --currentX;

                    if (currentX < 0)
                    {
                        currentX = Width - 1;
                        --currentY;
                    }
                }
            }

            // And clear all target positions after we populated the field.
            ClearAllTargetPositions();
            CurrentPlayer = attacker.Owner;
        }
    }
}
