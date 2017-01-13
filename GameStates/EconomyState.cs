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
            ecoGrid = new EconomyGrid(15, 15);

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
            // Handle input for the grid
            ecoGrid.HandleInput(helper, manager);

            // Update the overlay
            overlay.update(helper);
        }

        private void InitOverlay()
        {
            overlay.buttonEndTurn.ClickHandler = () => shouldTurnUpdate = true;
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

            overlay.labelCurrentPlayer.setLabelText("Current player: " + (currentPlayer ? "Blue" : "Red"));
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
