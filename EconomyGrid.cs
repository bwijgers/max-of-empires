using MaxOfEmpires.GameStates;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MaxOfEmpires.Buildings;
using System;

namespace MaxOfEmpires
{
    class EconomyGrid : Grid
    {
        private Point battlePosition;

        public EconomyGrid(int width, int height, List<Player> players, string id = "") : base(width, height, players, id)
        {
            foreach(Player p in players)
            {
                p.grid = this;
            }
            battlePosition = InvalidTile;
        }

        TimeSpan onTurnStart;

        public void Build(Builder builder, Building building)
        {
            builder.MovesLeft = 0;
            Tile t = this[builder.PositionInGrid] as Tile;
            t.Building = building;
            SelectTile(InvalidTile);
        }

        /// <summary>
        /// Initiates a battle between two Armies.
        /// </summary>
        /// <param name="attacker">The Army that initiated the attack.</param>
        /// <param name="defender">The Army that is being attacked.</param>
        private void InitBattle(Army attacker, Army defender, bool split = false)
        {
            // Save the square on which the battle is occuring (the defender's square)
            battlePosition = defender.PositionInGrid;

            // Remove BOTH armies from the grid; one will be replaced by what is remaining, the other will be annihilated
            Tile attackingTile = this[attacker.PositionInGrid] as Tile;
            Tile defendingTile = this[defender.PositionInGrid] as Tile;
            if(!split)
                (this[attacker.PositionInGrid] as Tile).SetUnit(null);
            (this[defender.PositionInGrid] as Tile).SetUnit(null);

            // Unselect the current tile as we move to another state
            SelectTile(InvalidTile);

            // Tell the battle state that we want to initiate a battle
            GameStateManager.OnInitiateBattle(attacker, defender, attackingTile, defendingTile);
        }

        public override void InitField()
        {
            base.InitField();
            BalancedEconomyGrid(Width, Height);
            foreach (Player p in players)
            {
                p.CalculatePopulation();
                p.CalculateMoneyPerTurn();
            }
        }

        public override void OnLeftClick(InputHelper helper)
        {
            // Get the current Tile under the mouse
            Tile clickedTile = GetTileUnderMouse(helper, true);

            // Do nothing if there is no clicked tile.
            if (clickedTile == null)
                return;

            // If the player had a tile selected and it contains a Unit...
            if (SelectedTile != null && SelectedTile.Occupied)
            {
                OnMoveUnit(SelectedTile, clickedTile);
            }

            // Check if the player clicked a tile with a Unit on it, and select it if it's there. 
            else if (clickedTile.Occupied && clickedTile.Unit.Owner == currentPlayer && !clickedTile.Unit.HasMoved)
            {
                // If the Unit can walk, show where it is allowed to walk. 
                walkablePositions = Pathfinding.ReachableTiles(clickedTile.Unit,Width,Height);
                SetUnitWalkingOverlay(walkablePositions);
                if (clickedTile.Unit is Army)
                    SetArmyAttackingOverlay((Army)clickedTile.Unit);

                // This unit can be selected. Show the player it is selected too
                SelectTile(clickedTile.PositionInGrid);
            }

            // Check if the player clicked a tile with a Building they own
            else if (clickedTile.BuiltOn && clickedTile.Building.Owner == currentPlayer)
            {
                // Select the building
                SelectTile(InvalidTile);
                SelectTile(clickedTile.PositionInGrid);
            }
        }

        private void OnMoveUnit(Tile selectedTile, Tile clickedTile)
        {
            bool mainUnit = true;
            if (selectedTile.Unit is Army && !(selectedTile.Unit as Army).AllUnitsSelected)
            {
                mainUnit = false;
            }
            // Check if we're attacking another player's army
            if (selectedTile.Unit is Army && IsAdjacent(clickedTile.PositionInGrid, selectedTile.PositionInGrid) && clickedTile.Occupied)
            {
                // If we're clicking on our own army, merge the armies
                if (selectedTile.Unit.Owner == clickedTile.Unit.Owner)
                {
                    // Merging with a non-army? No can do
                    if (!(clickedTile.Unit is Army))
                        return;

                    // Hey, let's merge the armies :)
                    if (mainUnit)
                    {
                        if((clickedTile.Unit as Army).MergeArmy((selectedTile.Unit as Army)))
                        {
                            selectedTile.SetUnit(null);
                        }
                        
                    }
                    else
                    {
                        Army selectedArmy = selectedTile.Unit as Army;
                        Army splitArmy = selectedArmy.SplitArmy(selectedArmy.SelectedUnits);
                        Army clickedArmy = (clickedTile.Unit as Army);
                        if (!clickedArmy.MergeArmy(splitArmy))
                        {
                            selectedArmy.MergeArmy(splitArmy);
                        }
                    }
                    SelectTile(InvalidTile);
                    return;

                }

                // If it's an enemy Builder, kill it and overwrite it
                Unit enemy = clickedTile.Unit;
                if (enemy is Builder)
                {
                    if (mainUnit)
                    {
                        selectedTile.Unit.MovesLeft -= 1;
                        clickedTile.SetUnit(null); // TODO: Test if this line can be removed
                        clickedTile.SetUnit(selectedTile.Unit);
                        selectedTile.SetUnit(null);
                    }
                    else
                    {
                        Army splitArmy = (selectedTile.Unit as Army).SplitArmy((selectedTile.Unit as Army).SelectedUnits);
                        splitArmy.MovesLeft -= 1;
                        clickedTile.SetUnit(null); // TODO: Test if this line can be removed
                        clickedTile.SetUnit(splitArmy);
                    }
                    SelectTile(InvalidTile);
                    return;
                }


                // Initiate a battle between armies
                if (mainUnit)
                {
                    InitBattle((Army)selectedTile.Unit, (Army)enemy);
                }
                else
                {
                    InitBattle((selectedTile.Unit as Army).SplitArmy((selectedTile.Unit as Army).SelectedUnits), (Army)enemy);
                }
                return;
            }

            // ... move the Army there, if the square is not occupied and the Army is capable, then unselect the tile.
            selectedTile.Unit.TargetPosition = clickedTile.PositionInGrid;
            Point movePos = Pathfinding.MoveTowardsTarget(selectedTile.Unit);

            if (movePos.Equals(selectedTile.Unit.PositionInGrid) || CheckMoveUnit(movePos, selectedTile.Unit))
            {
                SelectTile(InvalidTile);
                return;
            }
        }

        public override void TurnUpdate(uint turn, Player player,GameTime t)
        {
            base.TurnUpdate(turn, player,t);
            foreach(Player p in players)
            {
                if (p!= currentPlayer)
                {
                    if (t != null)
                    {
                        p.stats.money.Add(p.Money);
                        p.stats.population.Add(p.Population);
                        p.stats.buildings.Add(new Dictionary<string, int>());
                        p.stats.units.Add(new Dictionary<string, int>());
                        p.stats.lostUnits.Add(new Dictionary<string, int>());
                        p.stats.lostBuildings.Add(new Dictionary<string, int>());

                        if (onTurnStart == null)
                            p.stats.duration.Add(t.TotalGameTime);
                        else
                            p.stats.duration.Add(t.TotalGameTime - onTurnStart);
                        onTurnStart = t.TotalGameTime;
                    
                        ForEach(obj => {
                            Tile tile = (obj as Tile);
                            if (tile.BuiltOn && tile.Building.Owner == p)
                                p.AddBuildingToStats(tile.Building.id);
                            if(tile.Occupied && tile.Unit.Owner == p && tile.Unit is Army)
                                p.AddUnits((tile.Unit as Army).UnitsAndCounts);

                        });
                    }
                }
            }
        }

        /// <summary>
        /// Called from the battle grid when a player won a battle. 
        /// </summary>
        /// <param name="remainingArmy">The remaining army of the winning player after the battle.</param>
        public void OnPlayerWinBattle(Army remainingArmy)
        {
            remainingArmy.Owner.stats.battlesWon++;

            (this[battlePosition] as Tile).SetUnit(remainingArmy);
            battlePosition = InvalidTile;

            foreach(Player p in players)
            {
                if (p == remainingArmy.Owner)
                    p.stats.battlesWon++;
                else
                    p.stats.battlesLost++;
                p.CalculatePopulation();
            }
        }
    }
}
