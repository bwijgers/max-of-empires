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
        private GuiButton buttonStart;
        private GuiLabel labelTitle; 

        public OverlayMainMenu()
        {
            // Add the Game title
            labelTitle = GuiLabel.createNewLabel(new Vector2(10), "Max of Empires", "titleFont");
            addElement(labelTitle);

            // Quit button
            buttonQuit = GuiButton.createButtonWithLabel(new Point(0, MaxOfEmpires.ScreenSize.Y - 50), "Quit?", null, "font");
            buttonQuit.ClickHandler = () => { MaxOfEmpires.Quit(); };
            addElement(buttonQuit);

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
            labelTitle.move(new Point(MaxOfEmpires.ScreenSize.X / 2 - labelTitle.Bounds.Width / 2, labelTitle.Bounds.Y));

            // Buttons
            buttonQuit.move(new Point(MaxOfEmpires.ScreenSize.X / 2 - buttonQuit.Bounds.Width / 2, buttonQuit.Bounds.Y));
            buttonStart.move(new Point(MaxOfEmpires.ScreenSize.X / 2 - buttonStart.Bounds.Width / 2, buttonStart.Bounds.Y));
        }
    }
}
