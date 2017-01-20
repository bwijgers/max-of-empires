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
    class OverlaySettingsMenu : GuiScreen
    {
        private GuiLabel labelTitle;

        private GuiLabel labelCamera;
        private GuiButton buttonCameraControlMouse;
        private GuiButton buttonCameraControlKeyboard;
        private GuiButton buttonCameraControlBoth;

        private GuiLabel labelResolution;
        private GuiButton buttonResolution1;
        private GuiButton buttonResolution2;
        private GuiButton buttonResolution3;

        private GuiLabel labelFullscreen;
        private GuiButton buttonFullscreenOn;
        private GuiButton buttonFullscreenOff;
        
        private GuiButton buttonSave;
        private GuiButton buttonBack;
        private GuiButton buttonDefault;

        private string currentCamera;
        private string currentResolution;
        private string currentFullscreen;

        public OverlaySettingsMenu()
        {
            CheckCurrent();

            // Add the settings menu label
            labelTitle = GuiLabel.createNewLabel(new Vector2(10), "Settings", "titleFont");
            addElement(labelTitle);

            // Camera setting
            // Add the camera label
            labelCamera = GuiLabel.createNewLabel(new Vector2(100, 110), "How do you wish to control the camera? Current: " + CurrentCamera, "font");
            addElement(labelCamera);

            // Mouse control button
            buttonCameraControlMouse = GuiButton.createButtonWithLabel(new Point(100, 140), "Mouse", null, "font");
            buttonCameraControlMouse.ClickHandler = () => { MaxOfEmpires.settings.CameraControl = 1; CurrentCamera = "Mouse"; };
            addElement(buttonCameraControlMouse);

            // Keyboard control button
            buttonCameraControlKeyboard = GuiButton.createButtonWithLabel(new Point(200, 140), "Keyboard", null, "font");
            buttonCameraControlKeyboard.ClickHandler = () => { MaxOfEmpires.settings.CameraControl = 2; CurrentCamera = "Keyboard"; };
            addElement(buttonCameraControlKeyboard);

            // Both control button
            buttonCameraControlBoth = GuiButton.createButtonWithLabel(new Point(300, 140), "Both", null, "font");
            buttonCameraControlBoth.ClickHandler = () => { MaxOfEmpires.settings.CameraControl = 3; CurrentCamera = "Both"; };
            addElement(buttonCameraControlBoth);


            // Resolution setting
            // Add the resolution label
            labelResolution = GuiLabel.createNewLabel(new Vector2(100, 210), "What is your preferred resolution? Current: " + CurrentResolution, "font");
            addElement(labelResolution);

            // 800 x 480 button
            buttonResolution1 = GuiButton.createButtonWithLabel(new Point(100, 240), "800 x 480", null, "font");
            buttonResolution1.ClickHandler = () => { MaxOfEmpires.settings.Resolution = 1; CurrentResolution = "800 x 480"; };
            addElement(buttonResolution1);

            // 1280 x 768 button
            buttonResolution2 = GuiButton.createButtonWithLabel(new Point(200, 240), "1280 x 768", null, "font");
            buttonResolution2.ClickHandler = () => { MaxOfEmpires.settings.Resolution = 2; CurrentResolution = "1280 x 768"; }; 
            addElement(buttonResolution2);

            // 1920 x 1080 button
            buttonResolution3 = GuiButton.createButtonWithLabel(new Point(300, 240), "1920 x 1080", null, "font");
            buttonResolution3.ClickHandler = () => { MaxOfEmpires.settings.Resolution = 3; CurrentResolution = "1920 x 1080"; };
            addElement(buttonResolution3);


            // Fullscreen setting
            // Add the fullscreen label
            labelFullscreen = GuiLabel.createNewLabel(new Vector2(100, 310), "Would you like to play in fullscreen? Current: " + CurrentFullscreen, "font");
            addElement(labelFullscreen);

            // Fullscreen on button
            buttonFullscreenOn = GuiButton.createButtonWithLabel(new Point(100, 340), "Yes", null, "font");
            buttonFullscreenOn.ClickHandler = () => { MaxOfEmpires.settings.Fullscreen = true; CurrentFullscreen = "Yes"; };
            addElement(buttonFullscreenOn);

            // Fullscreen off button
            buttonFullscreenOff = GuiButton.createButtonWithLabel(new Point(200, 340), "No", null, "font");
            buttonFullscreenOff.ClickHandler = () => { MaxOfEmpires.settings.Fullscreen = false; CurrentFullscreen = "No"; };
            addElement(buttonFullscreenOff);


            // Operator buttons
            // Back button
            buttonBack = GuiButton.createButtonWithLabel(new Point(0, MaxOfEmpires.ScreenSize.Y - 50), "Back to main", null, "font");
            buttonBack.ClickHandler = () => { GameStateManager.SwitchState("mainMenu", true); MaxOfEmpires.settings.BackToSaved(); CheckCurrent(); };
            addElement(buttonBack);
            MoveToCenter(buttonBack);

            // Save button
            buttonSave = GuiButton.createButtonWithLabel(new Point((buttonBack.Bounds.X - (MaxOfEmpires.ScreenSize.X / 4)), MaxOfEmpires.ScreenSize.Y - 50), "Save and back", null, "font");
            buttonSave.ClickHandler = () => { MaxOfEmpires.settings.ApplySettings(); GameStateManager.SwitchState("mainMenu", true); };
            addElement(buttonSave);

            // Default buttton
            buttonDefault = GuiButton.createButtonWithLabel(new Point((buttonBack.Bounds.X + (MaxOfEmpires.ScreenSize.X / 4)), MaxOfEmpires.ScreenSize.Y - 50), "Reset to default", null, "font");
            buttonDefault.ClickHandler = () => { MaxOfEmpires.settings.ResetSettings(); CheckCurrent(); };
            addElement(buttonDefault);

            // Set everything to the center of the screen
            CenterElements();
        }

        private void CheckCurrent()
        {
            MaxOfEmpires.settings.SaveCurrentSettings();
            switch (MaxOfEmpires.settings.CameraControl)
            {
                case 1: currentCamera = "Mouse"; break;
                case 2: currentCamera = "Keyboard"; break;
                case 3: currentCamera = "Both"; break;
            }

            switch (MaxOfEmpires.settings.Resolution)
            {
                case 1: currentResolution = "800 x 480"; break;
                case 2: currentResolution = "1280 x 768"; break;
                case 3: currentResolution = "1920 x 1080"; break;
            }

            switch (MaxOfEmpires.settings.Fullscreen)
            {
                case true: currentFullscreen = "Yes"; break;
                case false: currentFullscreen = "No"; break;
            }
        }


        public void CenterElements()
        {
            // Title
            MoveToCenter(labelTitle);
            
            // Buttons
            MoveToCenter(buttonBack);
        }

        private void MoveToCenter(GuiElement elem)
        {
            elem.move(new Point(MaxOfEmpires.ScreenSize.X / 2 - elem.Bounds.Width / 2, elem.Bounds.Y));
        }

        public override void onMisclick()
        {
            
        }

        public string CurrentCamera
        {
            get
            {
                return currentCamera;
            }
            set
            {
                currentCamera = value;
                labelCamera.setLabelText("How do you wish to control the camera? Current: " + currentCamera);
            }
        }

        public string CurrentResolution
        {
            get
            {
                return currentResolution;
            }
            set
            {
                currentResolution = value;
                labelResolution.setLabelText("What is your preferred resolution? Current: " + currentResolution);
            }
        }

        public string CurrentFullscreen
        {
            get
            {
                return currentFullscreen;
            }
            set
            {
                currentFullscreen = value;
                labelFullscreen.setLabelText("Would you like to play in fullscreen? Current: " + currentFullscreen);
            }
        }

    }
}
