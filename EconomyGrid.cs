using MaxOfEmpires.GameStates;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MaxOfEmpires.Buildings;

namespace MaxOfEmpires
{
    class EconomyGrid : Grid
    {
        private Point battlePosition;

        public EconomyGrid(int width, int height, List<Player> players, string id = "") : base(width, height, players, id)
        {
            battlePosition = InvalidTile;
        }

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
        private void InitBattle(Army attacker, Army defender)
        {
            // Save the square on which the battle is occuring (the defender's square)
            battlePosition = defender.PositionInGrid;

            // Remove BOTH armies from the grid; one will be replaced by what is remaining, the other will be annihilated
            Tile attackingTile = this[attacker.PositionInGrid] as Tile;
            Tile defendingTile = this[defender.PositionInGrid] as Tile;
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
            EconomyGenerate();
            // Generate 2 armies and place them on the field.
            (this[0, 0] as Tile).SetUnit(new Builder(0, 0, players[0]));
            (this[14, 14] as Tile).SetUnit(new Builder(0, 0, players[1]));

            // Clear the Armies' targets
            ClearAllTargetPositions();
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
                Point[] walkablePositions = Pathfinding.ReachableTiles(clickedTile.Unit);
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
                SelectTile(clickedTile.PositionInGrid);
            }
        }

        private void OnMoveUnit(Tile selectedTile, Tile clickedTile)
        {
            // Check if we're attacking another player's army
            if (selectedTile.Unit is Army && IsAdjacent(clickedTile.PositionInGrid, selectedTile.PositionInGrid) && clickedTile.Occupied)
            {
                // If we're clicking on our own army, do nothing
                if (selectedTile.Unit.Owner == clickedTile.Unit.Owner)
                {
                    return;
                }

                // If it's an enemy Builder, kill it and overwrite it
                Unit enemy = clickedTile.Unit;
                if (enemy is Builder)
                {
                    selectedTile.Unit.MovesLeft -= 1;
                    clickedTile.SetUnit(null); // TODO: Test if this line can be removed
                    clickedTile.SetUnit(selectedTile.Unit);
                    selectedTile.SetUnit(null);
                    SelectTile(InvalidTile);
                    return;
                }

                // Initiate a battle between armies
                InitBattle((Army)selectedTile.Unit, (Army)enemy);
                return;
            }

            // ... move the Army there, if the square is not occupied and the Army is capable, then unselect the tile.
            selectedTile.Unit.TargetPosition = clickedTile.PositionInGrid;
            Point movePos = Pathfinding.MoveTowardsTarget(selectedTile.Unit);

            if (CheckMoveUnit(movePos, selectedTile.Unit))
            {
                SelectTile(InvalidTile);
                return;
            }
        }

        /// <summary>
        /// Called from the battle grid when a player won a battle. 
        /// </summary>
        /// <param name="remainingArmy">The remaining army of the winning player after the battle.</param>
        public void OnPlayerWinBattle(Army remainingArmy)
        {
            (this[battlePosition] as Tile).SetUnit(remainingArmy);
            battlePosition = InvalidTile;
        }
    }
}
