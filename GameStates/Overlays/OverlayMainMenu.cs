using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.GameStates.Overlays
{
    class OverlayMainMenu : GuiScreen
    {
        private GuiButton buttonQuit;
        private GuiButton buttonSure;
        private GuiButton buttonStart;
        private GuiLabel labelTitle; 

        public OverlayMainMenu()
        {
            // Add the Game title
            labelTitle = GuiLabel.createNewLabel(new Vector2(10), "Max of Empires", "titleFont");
            addElement(labelTitle);

            // Quit button
            buttonQuit = GuiButton.createButtonWithLabel(new Point(0, MaxOfEmpires.ScreenSize.Y - 50), "Quit?", null, "font");
            buttonQuit.ClickHandler = () => { buttonQuit.Visible = false; buttonSure.Visible = true; };
            addElement(buttonQuit);

            // Are you sure? button
            buttonSure = GuiButton.createButtonWithLabel(new Point(0, MaxOfEmpires.ScreenSize.Y - 50), "Are you sure?", null, "font");
            buttonSure.ClickHandler = () => { MaxOfEmpires.Quit(); };
            buttonSure.Visible = false;
            addElement(buttonSure);

            // Start button
            buttonStart = GuiButton.createButtonWithLabel(new Point(0, buttonQuit.Bounds.Y - 50), "Start Game", null, "font");
            buttonStart.ClickHandler = () => GameStateManager.SwitchState("battle");
            addElement(buttonStart);

            // Set everything to the center of the screen
            CenterElements();
        }

        public void CenterElements()
        {
            // Title
            MoveToCenter(labelTitle);

            // Buttons
            MoveToCenter(buttonQuit);
            MoveToCenter(buttonStart);
            MoveToCenter(buttonSure);
        }

        private void MoveToCenter(GuiElement elem)
        {
            elem.move(new Point(MaxOfEmpires.ScreenSize.X / 2 - elem.Bounds.Width / 2, elem.Bounds.Y));
        }
    }
}
