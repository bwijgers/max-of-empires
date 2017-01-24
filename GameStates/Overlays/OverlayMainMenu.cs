using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private GuiButton buttonSettings;
        private GuiButton buttonSure;

        public OverlayMainMenu()
        {
            // Quit button
            buttonQuit = GuiButton.createButtonWithLabel(new Point(0, MaxOfEmpires.ScreenSize.Y - 100), "Quit?", null, "font");
            buttonQuit.ClickHandler = () => { buttonQuit.Visible = false; buttonSure.Visible = true; };
            addElement(buttonQuit);

            // Are you sure? button
            buttonSure = GuiButton.createButtonWithLabel(new Point(0, MaxOfEmpires.ScreenSize.Y - 100), "Are you sure?", null, "font");
            buttonSure.ClickHandler = () => { MaxOfEmpires.Quit(); };
            buttonSure.Visible = false;
            addElement(buttonSure);

            // Settings button
            buttonSettings = GuiButton.createButtonWithLabel(new Point(0, buttonQuit.Bounds.Y - 100), "Settings", "TitleScreen/SettingsButton", "font");
            buttonSettings.ClickHandler = () => GameStateManager.SwitchState("settingsMenu", true);
            addElement(buttonSettings);

            // Start button
            buttonStart = GuiButton.createButtonWithLabel(new Point(0, buttonSettings.Bounds.Y - 100), "Start Game", null, "font");
            buttonStart.ClickHandler = () => GameStateManager.SwitchState("economy", true);
            addElement(buttonStart);

            // Set everything to the center of the screen
            CenterElements();
        }

        public void CenterElements()
        {
            // Buttons
            MoveToCenter(buttonQuit);
            MoveToCenter(buttonStart);
            MoveToCenter(buttonSure);
            MoveToCenter(buttonSettings);
        }

        private void MoveToCenter(GuiElement elem)
        {
            elem.move(new Point(MaxOfEmpires.ScreenSize.X / 2 - elem.Bounds.Width / 2, elem.Bounds.Y));
        }

        public override void onMisclick()
        {
            buttonQuit.Visible = true;
            buttonSure.Visible = false;
        }
    }
}
