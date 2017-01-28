using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System;
using MaxOfEmpires.Buildings;
using System.Collections.Generic;
using MaxOfEmpires.Files;

namespace MaxOfEmpires.GameStates.Overlays
{
    class OverlayEconomyState : GuiScreen
    {
        // Statics
        private static Point buildingInfoPosition;
        public static Point BuildingInfoPosition => buildingInfoPosition;

        // Buttons
        private GuiButton buttonEndTurn;

        // The currently selected builder and building, if applicable
        private Builder currentBuilder;
        private Building currentBuilding;

        private Army currentArmy;
        private bool refreshArmyInfo;

        // Labels
        private GuiLabel labelCurrentPlayer;
        private GuiLabel labelPlayerMoney;
        private GuiLabel labelPlayerMoneyPerTurn;
        private GuiLabel labelPlayerPopulation;

        // Lists
        private GuiList listArmySoldiers;
        private GuiList listBuilderActions;
        private GuiList listBuildingActions;

        // Overlay background color
        public Color playerColor;
        public Color oldColor;
        public double timeSinceSwitch;

        public OverlayEconomyState()
        {
            // Add the end turn button
            buttonEndTurn = GuiButton.createButtonWithLabel(MaxOfEmpires.overlayPos.ToPoint(), "End turn", null, "font");
            addElement(buttonEndTurn);

            // Add a label showing whose turn it currently is
            labelCurrentPlayer = GuiLabel.createNewLabel(new Vector2(buttonEndTurn.Bounds.Right + 2, buttonEndTurn.Bounds.Top + 2), "Current player: ", "font");
            addElement(labelCurrentPlayer);

            // Add a label telling the player how much money they have
            labelPlayerMoney = GuiLabel.createNewLabel(new Vector2(labelCurrentPlayer.Bounds.Left, labelCurrentPlayer.Bounds.Bottom + 5), "Money: ", "font");
            addElement(labelPlayerMoney);

            labelPlayerMoneyPerTurn = GuiLabel.createNewLabel(new Vector2(labelCurrentPlayer.Bounds.Left, labelPlayerMoney.Bounds.Bottom + 5), "Money per turn: ", "font");
            addElement(labelPlayerMoneyPerTurn);

            // Add a label telling the player how much population they have
            labelPlayerPopulation = GuiLabel.createNewLabel(new Vector2(labelCurrentPlayer.Bounds.Left, labelPlayerMoneyPerTurn.Bounds.Bottom + 5), "Free Population: 0", "font");
            addElement(labelPlayerPopulation);

            // Add labels for unit stats
            listArmySoldiers = GuiList.createNewList(new Point(labelPlayerMoneyPerTurn.Bounds.Location.X, labelPlayerPopulation.Bounds.Bottom + 5), 5, new List<GuiElement>(), 300);
            listArmySoldiers.addElement(ElementArmySelection.CreateBuildButton(Point.Zero, "1", null, null)); // Add this so that the size is calculated correctly
            addElement(listArmySoldiers);

            buildingInfoPosition = new Point(buttonEndTurn.Bounds.Left, listArmySoldiers.Bounds.Bottom + listArmySoldiers.MaxHeight + 5);

            // Remove this label so that it doesn't display bullshit :)
            listArmySoldiers.removeElement(0);
        }

        private void AddBuilderButton(EconomyGrid grid, GuiList listBuilderActions, string buildingName, Type buildingType)
        {
            StringBuilder label = new StringBuilder();
            label.Append(Translations.GetTranslation(buildingName)).Append(" (");
            label.Append(BuildingRegistry.GetCost(buildingName)).Append("G): ");
            listBuilderActions.addElement(ElementBuildButton.CreateBuildButton(listBuilderActions.Bounds.Location, label.ToString(), BuildBuilding(grid, buildingName, buildingType)));
        }

        private GuiButton.OnClickHandler BuildBuilding(EconomyGrid grid, string buildingName, Type buildingType)
        {
            // Return an on click handler
            return () => {
                // That gets the tile the current builder is on
                Tile t = grid[currentBuilder.PositionInGrid] as Tile;

                // Then checks if we can build here, and if the player can afford the building
                if (!t.BuiltOn && currentBuilder.Owner.CanAfford(BuildingRegistry.GetCost(buildingName)))
                {
                    Tile tile = currentBuilder.Parent as Tile;

                    if (!tile.Terrain.IsMountain && buildingType.Equals(typeof(Mine)))
                    {
                        return;
                    }
                    else if (tile.Terrain != Terrain.Plains && buildingType.Equals(typeof(Town)))
                    {
                        return;
                    }
                    else if ((tile.Terrain == Terrain.Lake || tile.Terrain == Terrain.Mountain || tile.Terrain == Terrain.DesertMountain || tile.Terrain == Terrain.TundraMountain)&& !buildingType.Equals(typeof(Mine)))
                    {
                        return;
                    }
                    // And remove the money if possible...
                    currentBuilder.Owner.Buy(BuildingRegistry.GetCost(buildingName));

                    // ... and tell the grid to build a building here, based on the passed type.
                    // Basically the same as new Building(currentBuilder.PositionInGrid, currentBuilder.Owner), except we don't need to know WHICH building
                    grid.Build(currentBuilder, (Building)Activator.CreateInstance(buildingType, new object[] { currentBuilder.PositionInGrid, currentBuilder.Owner }));

                    currentBuilder.Owner.CalculatePopulation();
                    currentBuilder.Owner.CalculateMoneyPerTurn();
                }
            };
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            Color drawColor;
            if (timeSinceSwitch <= 1)
            {
                drawColor = Color.Lerp(oldColor, playerColor, (float)timeSinceSwitch);
            }
            else
            {
                drawColor = playerColor;
            }

            DrawingHelper.Instance.DrawRectangle(spriteBatch, new Rectangle(MaxOfEmpires.overlayPos.ToPoint(), MaxOfEmpires.ScreenSize), drawColor);
            base.draw(spriteBatch);
        }

        public void InitBuildingList(EconomyGrid grid)
        {
            // Create the list of building possibilities
            listBuilderActions = GuiList.createNewList(BuildingInfoPosition, 5, new List<GuiElement>(), 300);

            // Add all the corresponding elements to the building actions list
            AddBuilderButton(grid, listBuilderActions, "building.town", typeof(Town));
            AddBuilderButton(grid, listBuilderActions, "building.mine", typeof(Mine));
            AddBuilderButton(grid, listBuilderActions, "building.trainingGrounds", typeof(TrainingGrounds));
            AddBuilderButton(grid, listBuilderActions, "building.academy", typeof(Academy));

            // Make sure the list knows how big it is and add it to the screen
            listBuilderActions.calculateElementPositions();
            addElement(listBuilderActions);

            // Create the building actions list and add it to the screen
            listBuildingActions = GuiList.createNewList(BuildingInfoPosition, 5, new List<GuiElement>(), 300);
            addElement(listBuildingActions);
        }

        /// <summary>
        /// Prints a Unit's information on the screen, or makes the information labels disappear if there is no Unit.
        /// </summary>
        /// <param name="a">The Army of which the information should be printed.</param>
        public void PrintArmyInfo(Army a)
        {
            // No Army? Make the info disappear :o
            if (a == null)
            {
                currentArmy = null;
                listArmySoldiers.Visible = false;
                return;
            }
            if (currentArmy != a || a.RefreshInfo)
            {
                // Stop refreshing the army's info 
                a.RefreshInfo = false;

                // Hey, we have an army to print :)
                if (currentArmy != a)
                {
                    listArmySoldiers.Visible = true;
                    currentArmy = a;
                }
                UpdateArmyInformation(currentArmy);
            }
        }

        public GuiButton.OnClickHandler LowerSelected(string s, Army a)
        {
            // Return an on click handler
            return () => {
                a.LowerSelected(s);
                a.RefreshInfo = true;
            };
        }

        public GuiButton.OnClickHandler AddSelected(string s, Army a)
        {
            // Return an on click handler
            return () => {
                a.AddSelected(s);
                a.RefreshInfo = true;
            };
        }

        public void PrintBuilderInfo(Builder builder)
        {
            currentBuilder = builder;
            listBuilderActions.Visible = builder != null;
        }

        public void PrintBuildingInfo(Building building)
        {
            currentBuilding = building;
            if (building != null)
            {
                listBuildingActions.clear();
                building.PopulateBuildingActions(listBuildingActions);
            }
            listBuildingActions.Visible = currentBuilding != null && listBuildingActions.AllLabels.Count > 0;
        }

        private void UpdateArmyInformation(Army a)
        {
            listArmySoldiers.clear();
            foreach (string soldierType in a.UnitsAndCounts.Keys)
            {
                // Create the label to add to the list
                StringBuilder sb = new StringBuilder();
                sb.Append(Translations.GetTranslation(soldierType));
                sb.Append(": ");
                sb.Append(a.SelectedUnits[soldierType] + "/");
                sb.Append(a.UnitsAndCounts[soldierType]);

                // Add this label to the list
                listArmySoldiers.addElement(ElementArmySelection.CreateBuildButton(Point.Zero, sb.ToString(), AddSelected(soldierType, a), LowerSelected(soldierType, a)));
            }
        }

        /// <summary>
        /// The function that is executed when the "End turn" button is pressed.
        /// </summary>
        public GuiButton.OnClickHandler EndTurnHandler
        {
            set
            {
                buttonEndTurn.ClickHandler = value;
            }
        }

        public GuiLabel LabelCurrentPlayer => labelCurrentPlayer;
        public GuiLabel LabelPlayerMoney => labelPlayerMoney;
        public GuiLabel LabelPlayerMoneyPerTurn => labelPlayerMoneyPerTurn;
        public GuiLabel LabelPlayerPopulation => labelPlayerPopulation;
    }
}