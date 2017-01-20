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
        private List<Player> players;
        private int currentPlayer;
        private EconomyGrid ecoGrid;
        private Overlays.OverlayEconomyState overlay;
        private bool shouldTurnUpdate;
        private uint turnNum;

        public EconomyState(Player blue, Player red)
        {
            players = new List<Player>();
            players.Add(blue);
            players.Add(red);
            currentPlayer = 0;

            ecoGrid = new EconomyGrid(15, 15, players);

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

            // Update the overlay
            overlay.update(helper);

            // Get the Unit under the mouse...
            Unit unitUnderMouse = ecoGrid.GetTileUnderMouse(helper)?.Unit;

            // ... and the selected Unit...
            Unit selectedUnit = ecoGrid.SelectedTile?.Unit;

            // Show nothing by default
            PrintArmyInfo(null);
            PrintBuilderInfo(null);

            // ... show the information of the currently selected Building...
            PrintBuildingInfo(ecoGrid.SelectedTile?.Building);

            // ... and print their information if it's an Army...
            if (unitUnderMouse is Army)
            {
                PrintArmyInfo((Army)unitUnderMouse);
                return;
            }
            if (selectedUnit is Army)
            {
                PrintArmyInfo((Army)selectedUnit);
                return;
            }

            // ... or set the building information if it's a Builder... 
            if (selectedUnit is Builder && !ecoGrid.SelectedTile.BuiltOn)
            {
                PrintBuilderInfo((Builder)selectedUnit);
                return;
            }

            if (manager.KeyPressed("nextTurn", helper))
            {
                shouldTurnUpdate = true;
            }
        }

        private void InitOverlay()
        {
            overlay.EndTurnHandler = () => shouldTurnUpdate = true;
            overlay.InitBuildingList(ecoGrid);

            foreach (Player p in players)
            {
                p.OnUpdateMoney(UpdateMoneyDisplay);
                p.OnUpdatePopulation(UpdatePopulationDisplay);
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

        private void PrintArmyInfo(Army a)
        {
            overlay.PrintArmyInfo(a);
        }

        private void PrintBuilderInfo(Builder builder)
        {
            overlay.PrintBuilderInfo(builder);
        }

        private void PrintBuildingInfo(Buildings.Building building)
        {
            overlay.PrintBuildingInfo(building);
        }

        public override void Reset()
        {
            // Initialize the field
            ecoGrid.InitField();

            // Player 1 starts
            currentPlayer = 0;

            // Turn number starts at 1, so 0 and double TurnUpdate
            turnNum = 0;
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
            SelectNextPlayer();
            ecoGrid.TurnUpdate(turnNum, CurrentPlayer);

            overlay.LabelCurrentPlayer.setLabelText("Current player: " + CurrentPlayer.Name);
            UpdateMoneyDisplay(CurrentPlayer);
            UpdatePopulationDisplay(CurrentPlayer);
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

        private void UpdateMoneyDisplay(Player p)
        {
            overlay.LabelPlayerMoney.setLabelText("Money: " + p.Money + "G");
        }

        public void UpdatePopulationDisplay(Player p)
        {
            overlay.LabelPlayerPopulation.setLabelText("Free Population: " + p.Population);
        }

        private Player CurrentPlayer => players[currentPlayer];
    }
}
