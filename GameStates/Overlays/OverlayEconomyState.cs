using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System;
using MaxOfEmpires.Buildings;
using System.Collections.Generic;

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

        // Labels
        private GuiLabel labelCurrentPlayer;
        private GuiLabel labelPlayerMoney;

        // Lists
        private GuiList listArmySoldiers;
        private GuiList listBuilderActions;
        private GuiList listBuildingActions;

        public OverlayEconomyState()
        {
            // Add the end turn button
            buttonEndTurn = GuiButton.createButtonWithLabel(new Point(500, 10), "End turn", null, "font");
            addElement(buttonEndTurn);

            // Add a label showing whose turn it currently is
            labelCurrentPlayer = GuiLabel.createNewLabel(new Vector2(buttonEndTurn.Bounds.Right + 2, buttonEndTurn.Bounds.Top + 2), "Current player: ", "font");
            addElement(labelCurrentPlayer);

            // Add a label telling the player how much money they have
            labelPlayerMoney = GuiLabel.createNewLabel(new Vector2(labelCurrentPlayer.Bounds.Left, labelCurrentPlayer.Bounds.Bottom + 5), "Money: ", "font");
            addElement(labelPlayerMoney);

            // Add labels for unit stats
            listArmySoldiers = GuiList.createNewList(new Point(labelPlayerMoney.Bounds.Location.X, labelPlayerMoney.Bounds.Bottom + 5), 5, new List<GuiElement>(), 300);
            addElement(listArmySoldiers);

            buildingInfoPosition = new Point(buttonEndTurn.Bounds.Left, listArmySoldiers.Bounds.Bottom + listArmySoldiers.MaxHeight + 5);
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
                    // And remove the money if possible...
                    currentBuilder.Owner.Buy(BuildingRegistry.GetCost(buildingName));

                    // ... and tell the grid to build a building here, based on the passed type.
                    // Basically the same as new Building(currentBuilder.PositionInGrid, currentBuilder.Owner), except we don't need to know WHICH building
                    grid.Build(currentBuilder, (Building)Activator.CreateInstance(buildingType, new object[] { currentBuilder.PositionInGrid, currentBuilder.Owner }));
                }
            };
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            DrawingHelper.Instance.DrawRectangle(spriteBatch, new Rectangle(480, 0, MaxOfEmpires.ScreenSize.X, MaxOfEmpires.ScreenSize.Y), Color.DeepPink);
            base.draw(spriteBatch);
        }

        public void InitBuildingList(EconomyGrid grid)
        {
            // Create the list of building possibilities
            listBuilderActions = GuiList.createNewList(BuildingInfoPosition, 5, new List<GuiElement>(), 300);

            // Add all the corresponding elements to the building actions list
            listBuilderActions.addElement(ElementBuildButton.CreateBuildButton(listBuilderActions.Bounds.Location, "Mine (100G): ", BuildBuilding(grid, "mine", typeof(Mine)))); // TODO: load cost number from somewhere
            listBuilderActions.addElement(ElementBuildButton.CreateBuildButton(listBuilderActions.Bounds.Location, "Training Grounds (150G): ", BuildBuilding(grid, "trainingGrounds", typeof(TrainingGrounds))));

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
                listArmySoldiers.Visible = false;
                return;
            }

            // Hey, we have an army to print :)
            listArmySoldiers.Visible = true;
            listArmySoldiers.clear();
            foreach (string soldierType in a.UnitsAndCounts.Keys)
            {
                // Create the label to add to the list
                StringBuilder sb = new StringBuilder();
                sb.Append(soldierType);
                sb.Append(": ");
                sb.Append(a.UnitsAndCounts[soldierType]);

                // Add this label to the list
                listArmySoldiers.addElement(GuiLabel.createNewLabel(new Vector2(), sb.ToString(), "font"));
            }
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
    }
}