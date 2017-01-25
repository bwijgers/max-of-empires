using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using MaxOfEmpires.Files;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameStates.Overlays
{
    class OverlayBattleState : GuiScreen
    {
        private GuiButton buttonEndTurn;
        private GuiLabel labelCurrentPlayer;
        private GuiLabel labelUnitAtt;
        private GuiLabel labelUnitHit;
        private GuiLabel labelUnitHp;
        private GuiLabel labelUnitName;

        // Overlay background color
        public Color playerColor;
        public Color oldColor;
        public double timeSinceSwitch;

        public OverlayBattleState()
        {
            // Add the end turn button
            buttonEndTurn = GuiButton.createButtonWithLabel(new Point((int)MaxOfEmpires.OverlayPos.X + 20, 10), "End turn", null, "font");
            addElement(buttonEndTurn);

            // Add a label showing whose turn it currently is
            labelCurrentPlayer = GuiLabel.createNewLabel(new Vector2(buttonEndTurn.Bounds.Right + 2, buttonEndTurn.Bounds.Top + 2), "Current player: ", "font");
            addElement(labelCurrentPlayer);

            // Add labels for unit stats
            labelUnitName = GuiLabel.createNewLabel(new Vector2(buttonEndTurn.Bounds.Left, buttonEndTurn.Bounds.Bottom + 100), "Type: ", "font");
            labelUnitHp = GuiLabel.createNewLabel(new Vector2(buttonEndTurn.Bounds.Left, labelUnitName.Bounds.Bottom + 2), "Unit Hp/Max: ", "font");
            labelUnitAtt = GuiLabel.createNewLabel(new Vector2(labelUnitHp.Bounds.Left, labelUnitHp.Bounds.Bottom + 2), "Unit Att/Def: ", "font");
            labelUnitHit = GuiLabel.createNewLabel(new Vector2(labelUnitAtt.Bounds.Left, labelUnitAtt.Bounds.Bottom + 2), "Unit Hit/Dodge: ", "font");

            addElement(labelUnitName);
            addElement(labelUnitHp);
            addElement(labelUnitAtt);
            addElement(labelUnitHit);
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

        /// <summary>
        /// Prints a Unit's information on the screen, or makes the information labels disappear if there is no Unit.
        /// </summary>
        /// <param name="u">The Unit of which the information should be printed.</param>
        public void PrintSoldierInfo(Soldier u)
        {
            // No Unit? Make the unit info disappear :o
            if(u == null)
            {
                labelUnitHp.Visible = labelUnitAtt.Visible = labelUnitHit.Visible = labelUnitName.Visible = false;
                return;
            }

            // There is a Unit? Show its stats. 
            labelUnitHp.Visible = labelUnitAtt.Visible = labelUnitHit.Visible = labelUnitName.Visible = true;
            labelUnitName.setLabelText("Type: " + Translations.GetTranslation(u.Name));
            labelUnitHp.setLabelText("Unit HP/Max: " + u.Stats.hp + '/' + u.Stats.maxHp);
            labelUnitAtt.setLabelText("Unit Att/Def: " + u.Stats.att + '/' + u.Stats.def);
            labelUnitHit.setLabelText("Unit Hit/Dodge: " + u.Stats.hit + '/' + u.Stats.dodge);
        }

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
