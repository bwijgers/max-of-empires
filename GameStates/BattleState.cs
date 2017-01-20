using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MaxOfEmpires.Units;
using System.Collections.Generic;

namespace MaxOfEmpires.GameStates
{
    
   class BattleState : GameState
    {
        private List<Player> players;
        private int currentPlayer;

        /// <summary>
        /// The battlegrid.
        /// </summary>
        private BattleGrid battleGrid;

        /// <summary>
        /// The overlay. Things like buttons are defined here. everything other than the grid is in here.
        /// </summary>
        private Overlays.OverlayBattleState overlay;
        private bool shouldTurnUpdate; // True if the next update should initiate a turn update.

        /// <summary>
        /// The current turn in this battle.
        /// </summary>
        private uint turnNum;

        public BattleState(Player blue, Player red)
        {
            // Initialize players
            players = new List<Player>();
            players.Add(blue);
            players.Add(red);
            currentPlayer = 0;

            // Initialize the battlefield.
            battleGrid = new BattleGrid(15, 15, players);
            battleGrid.InitField();

            // Initialize the field
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

        public override Vector2 GetCurrentGridSize()
        {
            int x = battleGrid.Width;
            int y = battleGrid.Height;
            return new Vector2(x, y);
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
                overlay.PrintSoldierInfo(u);
            }
            // if there is no selected Unit...
            else
            {
                // ... get the tile the mouse is over, and show the Unit's information, if this Unit exists. 
                Tile t = battleGrid.GetTileUnderMouse(helper);
                if (t != null)
                {
                    overlay.PrintSoldierInfo((Soldier) t.Unit);
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
            overlay.EndTurnHandler = () => { shouldTurnUpdate = true; };
        }

        /// <summary>
        /// Called when a player initiates a battle from the Economy state.
        /// </summary>
        /// <param name="attackingArmy">The army that attacked.</param>
        /// <param name="defendingArmy">The army that was attacked.</param>
        public void OnInitiateBattle(Army attackingArmy, Army defendingArmy, Tile attackingTile, Tile defendingTile)
        {
            battleGrid.InitField();
            battleGrid.BattleGenerate(attackingTile.Terrain, attackingTile.hills, defendingTile.Terrain, defendingTile.hills);
            battleGrid.PopulateField(attackingArmy, defendingArmy);
        }

        public override void Reset()
        {
            // Player 1 starts.
            currentPlayer = 0;

            // Turn number starts at 1.
            turnNum = 0;

            // Start turn
            TurnUpdate();
            TurnUpdate();
        }

        private void SelectNextPlayer()
        {
            ++currentPlayer;
            if (currentPlayer >= players.Count)
            {
                currentPlayer = 0;
                ++turnNum;
            }
        }

        /// <summary>
        /// Called when the turn is updated. Sets the current player to the other player and then calls Grid.TurnUpdate.
        /// </summary>
        public void TurnUpdate()
        {
            // Change the current player
            SelectNextPlayer();

            // TurnUpdate the grid
            battleGrid.TurnUpdate(turnNum, CurrentPlayer);

            // Show whose turn it is in the overlay
            overlay.LabelCurrentPlayer.setLabelText("Current player: " + CurrentPlayer.Name);
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
        
        /// <summary>
        /// Accessor for the BattleGrid.
        /// </summary>
        public BattleGrid BattleGrid => battleGrid;

        public Player CurrentPlayer => players[currentPlayer];
    }
}
