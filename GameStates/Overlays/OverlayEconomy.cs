using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace MaxOfEmpires.GameStates.Overlays
{
    class OverlayEconomyState : GuiScreen
    {
        private GuiButton buttonEndTurn;
        private GuiLabel labelCurrentPlayer;
        private GuiList listArmySoldiers;

        public OverlayEconomyState()
        {
            // Add the end turn button
            buttonEndTurn = GuiButton.createButtonWithLabel(new Point((int)MaxOfEmpires.OverlayPos.X + 20, 10), "End turn", null, "font");
            addElement(buttonEndTurn);

            // Add a label showing whose turn it currently is
            labelCurrentPlayer = GuiLabel.createNewLabel(new Vector2(buttonEndTurn.Bounds.Right + 2, buttonEndTurn.Bounds.Top + 2), "Current player: ", "font");
            addElement(labelCurrentPlayer);

            // Add labels for unit stats
            listArmySoldiers = GuiList.createNewList(new Point(buttonEndTurn.Bounds.Location.X, labelCurrentPlayer.Bounds.Bottom + 5), 5, new System.Collections.Generic.List<GuiLabel>(), 300);

            addElement(listArmySoldiers);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            DrawingHelper.Instance.DrawRectangle(spriteBatch, new Rectangle((int)MaxOfEmpires.OverlayPos.X, 0, MaxOfEmpires.ScreenSize.X, MaxOfEmpires.ScreenSize.Y), Color.DeepPink);
            base.draw(spriteBatch);
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
                listArmySoldiers.addLabel(GuiLabel.createNewLabel(new Vector2(), sb.ToString(), "font"));
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
    }
}