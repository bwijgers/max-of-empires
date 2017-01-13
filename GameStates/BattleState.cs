using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Ebilkill.Gui.Elements;
using MaxOfEmpires.Units;

namespace MaxOfEmpires.GameStates
{
    
   class BattleState : GameState
    {

        public override Vector2 GetCurrentGridSize()
        {
            int x = battleGrid.Width;
            int y = battleGrid.Height;
            return new Vector2(x, y);
        }

        /// <summary>
        /// The battlegrid.
        /// </summary>
        private BattleGrid battleGrid;

        /// <summary>
        /// The overlay. Things like buttons are defined here. everything other than the grid is in here.
        /// </summary>
        private Overlays.OverlayBattleState overlay;

        /// <summary>
        /// The current turn in this battle.
        /// </summary>
        private uint turnNum;

        private bool currentPlayer; // false if player 2, true if player 1.
        private bool shouldTurnUpdate; // True if the next update should initiate a turn update.

        public BattleState()
        {
            // Initialize the battlefield.
            battleGrid = new BattleGrid(15, 15);
            battleGrid.InitField();

            // Initialize the overlay.
            overlay = new Overlays.OverlayBattleState();
            InitOverlay();

            // Reset ourselves
            Reset();
        }

        public override void Draw(GameTime time, SpriteBatch gameObjectS, SpriteBatch overlayS)
        {
            battleGrid.Draw(time, gameObjectS);
            overlay.draw(overlayS);
            base.Draw(time, gameObjectS, overlayS);
        }

        public override void HandleInput(InputHelper helper, KeyManager manager)
        {
            // Handle input for the grid (things like moving and attacking units)
            battleGrid.HandleInput(helper, manager);

            // Get the selected Unit
            Soldier u = (Soldier) battleGrid.SelectedTile?.Unit;

            // Print the selected Unit's information, if it exists
            if (u != null)
            {
                overlay.PrintUnitInfo(u);
            }
            // if there is no selected Unit...
            else
            {
                // ... get the tile the mouse is over, and show the Unit's information, if this Unit exists. 
                Tile t = battleGrid.GetTileUnderMouse(helper);
                if (t != null)
                {
                    overlay.PrintUnitInfo((Soldier) t.Unit);
                }
            }

            // Update the overlay
            overlay.update(helper);
        }

        /// <summary>
        /// Called when a player initiates a battle from the Economy state.
        /// </summary>
        /// <param name="attackingArmy">The army that attacked.</param>
        /// <param name="defendingArmy">The army that was attacked.</param>
        public void OnInitiateBattle(Army attackingArmy, Army defendingArmy)
        {
            battleGrid.InitField();
            battleGrid.PopulateField(attackingArmy, defendingArmy);
        }

        /// <summary>
        /// Initializes the overlay. Add all GuiElements here.
        /// </summary>
        private void InitOverlay()
        {
            // Add a click handler to the end turn button.
            overlay.buttonEndTurn.ClickHandler = () => { shouldTurnUpdate = true; };
        }

        /// <summary>
        /// Called when the turn is updated. Sets the current player to the other player and then calls Grid.TurnUpdate.
        /// </summary>
        public void TurnUpdate()
        {
            // Change the current player
            currentPlayer = !currentPlayer;

            // Increase the turn number when the blue player starts their turn
            if (currentPlayer)
                ++turnNum;

            // TurnUpdate the grid
            battleGrid.TurnUpdate(turnNum, currentPlayer);

            // Show whose turn it is in the overlay
            overlay.labelCurrentPlayer.setLabelText("Current player: " + (currentPlayer ? "Blue" : "Red"));
        }

        public override void Update(GameTime time)
        {
            // Update fade in / fade out
            base.Update(time);

            // Update the grid
            battleGrid.Update(time);

            // TurnUpdate when requested.
            if (shouldTurnUpdate)
            {
                shouldTurnUpdate = false;
                TurnUpdate();
            }
        }

        public override void Reset()
        {
            // Player 1 starts.
            currentPlayer = true;

            // Turn number starts at 1.
            turnNum = 0;

            // Start turn
            TurnUpdate();
            TurnUpdate();
        }

        /// <summary>
        /// Accessor for the BattleGrid.
        /// </summary>
        public BattleGrid BattleGrid => battleGrid;
    }
}
