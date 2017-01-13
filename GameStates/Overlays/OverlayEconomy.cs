using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameStates.Overlays
{
    class OverlayEconomyState : GuiScreen
    {
        public GuiLabel labelCurrentPlayer;
        public GuiLabel labelUnitHp;
        public GuiLabel labelUnitAtt;
        public GuiLabel labelUnitHit;
        public GuiButton buttonEndTurn;

        public OverlayEconomyState()
        {
            // Add the end turn button
            buttonEndTurn = GuiButton.createButtonWithLabel(new Point(500, 10), "End turn", null, "font");
            addElement(buttonEndTurn);

            // Add a label showing whose turn it currently is
            labelCurrentPlayer = GuiLabel.createNewLabel(new Vector2(buttonEndTurn.Bounds.Right + 2, buttonEndTurn.Bounds.Top + 2), "Current player: ", "font");
            addElement(labelCurrentPlayer);

            // Add labels for unit stats
            labelUnitHp = GuiLabel.createNewLabel(new Vector2(buttonEndTurn.Bounds.Left, buttonEndTurn.Bounds.Bottom + 100), "Unit Hp/Max: ", "font");
            labelUnitAtt = GuiLabel.createNewLabel(new Vector2(labelUnitHp.Bounds.Left, labelUnitHp.Bounds.Bottom + 2), "Unit Att/Def: ", "font");
            labelUnitHit = GuiLabel.createNewLabel(new Vector2(labelUnitAtt.Bounds.Left, labelUnitAtt.Bounds.Bottom + 2), "Unit Hit/Dodge: ", "font");

            addElement(labelUnitHp);
            addElement(labelUnitAtt);
            addElement(labelUnitHit);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            DrawingHelper.Instance.DrawRectangle(spriteBatch, new Rectangle(480, 0, MaxOfEmpires.ScreenSize.X, MaxOfEmpires.ScreenSize.Y), Color.DeepPink);
            base.draw(spriteBatch);
        }

        /// <summary>
        /// Prints a Unit's information on the screen, or makes the information labels disappear if there is no Unit.
        /// </summary>
        /// <param name="u">The Unit of which the information should be printed.</param>
        public void PrintArmyInfo(Army u)
        {
            // No Unit? Make the unit info disappear :o
            if (u == null)
            {
                labelUnitHp.Visible = labelUnitAtt.Visible = labelUnitHit.Visible = false;
                return;
            }
        }
    }
}