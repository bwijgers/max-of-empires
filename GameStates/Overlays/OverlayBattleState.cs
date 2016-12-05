using Ebilkill.Gui.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.GameStates.Overlays
{
    class OverlayBattleState : Ebilkill.Gui.GuiScreen
    {
        public GuiLabel labelCurrentPlayer;
        public GuiButton buttonEndTurn;

        public OverlayBattleState()
        {
            // Add the end turn button
            buttonEndTurn = GuiButton.createButtonWithLabel(new Point(500, 10), "End turn", null, "font");
            addElement(buttonEndTurn);

            // Add a label showing whose turn it currently is
            labelCurrentPlayer = GuiLabel.createNewLabel(new Vector2(buttonEndTurn.Bounds.Right + 2, buttonEndTurn.Bounds.Top + 2), "Current player: ", "font");
            addElement(labelCurrentPlayer);
        }
    }
}
