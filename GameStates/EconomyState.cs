using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameStates
{
    class EconomyState : GameState
    {
        private EconomyGrid ecoGrid;

        private Overlays.OverlayEconomyState overlay; 

        private uint turnNum;
        private bool currentPlayer;
        private bool shouldTurnUpdate;

        public EconomyState()
        {
            ecoGrid = new EconomyGrid(MaxOfEmpires.minGridSize.X, MaxOfEmpires.minGridSize.Y);

            overlay = new Overlays.OverlayEconomyState();
            InitOverlay();
            Reset();
        }

        public override void Draw(GameTime time, SpriteBatch gameObjectS, SpriteBatch overlayS)
        {
            // Draw the economy grid and then the overlay, followed by possible fading (in/out)
            ecoGrid.Draw(time, gameObjectS);
            overlay.draw(overlayS);
            base.Draw(time, gameObjectS, overlayS);
        }

        public override Vector2 GetCurrentGridSize()
        {
            return new Vector2(ecoGrid.Width, ecoGrid.Height);
        }


        public override void HandleInput(InputHelper helper, KeyManager manager)
        {
            // Handle input for the grid (things like moving and attacking units)
            ecoGrid.HandleInput(helper, manager);

            // Get the selected Unit
            Army u = (Army)ecoGrid.SelectedTile?.Unit;

            // Print the selected Unit's information, if it exists
            if (u != null)
            {
                overlay.PrintArmyInfo(u);
            }
            // if there is no selected Unit...
            else
            {
                // ... get the tile the mouse is over, and show the Unit's information, if this Unit exists. 
                Tile t = ecoGrid.GetTileUnderMouse(helper);
                if (t != null)
                {
                    overlay.PrintArmyInfo((Army)t.Unit);
                }
            }

            // Update the overlay
            overlay.update(helper);
        }

        private void InitOverlay()
        {
            overlay.EndTurnHandler = () => shouldTurnUpdate = true;
        }

        /// <summary>
        /// Called when the turn is updated. Sets the current player to the other player and then calls Grid.TurnUpdate.
        /// </summary>
        public void TurnUpdate()
        {
            currentPlayer = !currentPlayer;
            if (currentPlayer)
                ++turnNum;
            ecoGrid.TurnUpdate(turnNum, currentPlayer);

            overlay.LabelCurrentPlayer.setLabelText("Current player: " + (currentPlayer ? "Blue" : "Red"));
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            ecoGrid.Update(time);
            if (shouldTurnUpdate)
            {
                shouldTurnUpdate = false;
                TurnUpdate();
            }
        }

        /// <summary>
        /// Called when a player wins a battle in the battle state.
        /// </summary>
        /// <param name="remainingArmy">The remaining army after the battle has ended.</param>
        public void OnPlayerWinBattle(Army remainingArmy)
        {
            ecoGrid.OnPlayerWinBattle(remainingArmy);
        }

        public override void Reset()
        {
            // Initialize the field
            ecoGrid.InitField();

            // Player 1 starts
            currentPlayer = true;

            // Turn number starts at 1, so 0 and double TurnUpdate
            turnNum = 0;
            TurnUpdate();
            TurnUpdate();
        }
    }
}
