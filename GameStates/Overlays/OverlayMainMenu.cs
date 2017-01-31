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
        private GuiLabel demoLabel;
        private Tile tile1 = new Tile(Terrain.Forest, 1, 1);
        private Tile tile2 = new Tile(Terrain.Plains, 1, 2);

        public OverlayMainMenu(Player blue, Player red)
        {
            demoLabel = GuiLabel.createNewLabel(new Vector2(10), "Demo", "titleFont");
            addElement(demoLabel);

            // Quit button
            buttonQuit = new GuiButton(new Rectangle(new Point(0, MaxOfEmpires.ScreenSize.Y - 200), new Point(300, 200)), "TitleScreen/QuitButton");
            buttonQuit.ClickHandler = () => { buttonQuit.Visible = false; buttonSure.Visible = true; };
            addElement(buttonQuit);

            // Are you sure? button
            buttonSure = new GuiButton(buttonQuit.Bounds, "TitleScreen/AreYouSureButton");
            buttonSure.ClickHandler = () => { MaxOfEmpires.Quit(); };
            buttonSure.Visible = false;
            addElement(buttonSure);

            // Settings button
            buttonSettings = new GuiButton(new Rectangle(new Point(0, buttonQuit.Bounds.Y - 150), new Point(300, 200)), "TitleScreen/SettingsButton");
            buttonSettings.ClickHandler = () => GameStateManager.SwitchState("settingsMenu", true);
            addElement(buttonSettings);

            // Start button
            buttonStart = new GuiButton(new Rectangle(new Point(0, buttonSettings.Bounds.Y - 150), new Point(300, 200)), "TitleScreen/StartGameButton");
            buttonStart.ClickHandler = () => GameStateManager.OnInitiateBattle(Units.Army.GenerateArmy(blue), Units.Army.GenerateArmy(red), tile1, tile2);
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
