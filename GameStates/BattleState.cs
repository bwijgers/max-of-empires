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
        /// <summary>
        /// The battlegrid.
        /// </summary>
        private Grid battleGrid;

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
            battleGrid = new Grid(15, 15);
            battleGrid.InitField();

            // Initialize the overlay.
            overlay = new Overlays.OverlayBattleState();
            InitOverlay();

            // Player 1 starts.
            currentPlayer = true;

            // Turn number starts at 1.
            turnNum = 1;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            battleGrid.Draw(time, s);
            overlay.draw(s);
        }

        public override void HandleInput(InputHelper helper, KeyManager manager)
        {
            // Handle input for the grid (things like moving and attacking units)
            battleGrid.HandleInput(helper, manager);

            // Get the selected Unit
            Unit u = battleGrid.SelectedTile?.Unit;

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
                    overlay.PrintUnitInfo(t.Unit);
                }
            }

            // Update the overlay
            overlay.update(helper);
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
            currentPlayer = !currentPlayer;
            battleGrid.TurnUpdate(turnNum, currentPlayer);

            overlay.labelCurrentPlayer.setLabelText("Current player: " + (currentPlayer ? "Blue" : "Red"));
        }

        public override void Update(GameTime time)
        {
            battleGrid.Update(time);
            if (shouldTurnUpdate)
            {
                shouldTurnUpdate = false;
                TurnUpdate();
            }
        }

        /// <summary>
        /// Accessor for the BattleGrid.
        /// </summary>
        public Grid BattleGrid => battleGrid;
    }
}
