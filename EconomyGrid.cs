using MaxOfEmpires.GameStates;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires
{
    class EconomyGrid : Grid
    {
        public EconomyGrid(int width, int height, string id = "") : base(width, height, id)
        {
        }

        public override void InitField()
        {
            // Set all the tiles in the field to Terrain.Plains
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    this[x, y] = new Tile(Terrain.Plains, x, y);
                }
            }

            // Generate 2 armies and place them on the field.
            (this[1, 1] as Tile).SetUnit(Army.GenerateArmy(true));
            (this[5, 5] as Tile).SetUnit(Army.GenerateArmy(false));

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

            // If the player had a tile selected and it contains an Army...
            if (SelectedTile != null && SelectedTile.Occupied)
            {
                // Check if we're attacking another player's army
                if (IsAdjacent(clickedTile.GridPos, SelectedTile.GridPos) && clickedTile.Occupied)
                {
                    // If we're clicking on our own army, do nothing
                    if (SelectedTile.Unit.Owner == clickedTile.Unit.Owner)
                    {
                        return;
                    }

                    // Initiate a battle between armies
                    InitBattle((Army) SelectedTile.Unit, (Army) clickedTile.Unit);
                }

                // ... move the Army there, if the square is not occupied and the Army is capable, then unselect the tile.
                SelectedTile.Unit.TargetPosition = clickedTile.GridPos;
                Point movePos = Pathfinding.MoveTowardsTarget(SelectedTile.Unit);

                if (CheckMoveUnit(movePos, SelectedTile.Unit))
                {
                    SelectTile(InvalidTile);
                    return;
                }
            }

            // Check if the player clicked a tile with a Unit on it, and select it if it's there. 
            else if (clickedTile.Occupied && clickedTile.Unit.Owner == currentPlayer && !clickedTile.Unit.HasMoved)
            {
                // If the Unit can walk, show where it is allowed to walk. 
                if (!clickedTile.Unit.HasMoved)
                {
                    Point[] walkablePositions = Pathfinding.ReachableTiles(clickedTile.Unit);
                    SetUnitWalkingOverlay(walkablePositions);
                    SetArmyAttackingOverlay((Army) clickedTile.Unit);
                }

                // This unit can be selected. Show the player it is selected too
                SelectTile(clickedTile.GridPos);
            }
        }

        /// <summary>
        /// Initiates a battle between two Armies.
        /// </summary>
        /// <param name="attacker">The Army that initiated the attack.</param>
        /// <param name="defender">The Army that is being attacked.</param>
        private void InitBattle(Army attacker, Army defender)
        {
            // Get the battle state
            BattleState state = (BattleState) GameStateManager.GetState("battle");

            // Create the new battle grid
            state.BattleGrid.InitField();
            state.BattleGrid.PopulateField(attacker, defender);

            // Switch to the battle state
            GameStateManager.SwitchState("battle", true);
        }
    }
}
