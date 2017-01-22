using MaxOfEmpires.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires
{
    public class Settings
    {
        private int cameraControl = 3;
        private int resolution = 2;
        private bool fullscreen = false;
        private int prevCameraControl;
        private int prevResolution;
        private bool prevFullscreen;

        public Settings()
        {
            LoadSettingsFromFile();
        }

        public void InitializeSettingsFromFile()
        {
            LoadSettingsFromFile();
            ApplySettings();
        }

        /// <summary>
        /// Loads the settings from the Settings.cfg file
        /// </summary>
        private void LoadSettingsFromFile()
        {
            Configuration settingsFile = FileManager.LoadConfig("Settings");
            cameraControl = settingsFile.GetProperty<int>("cameracontrol");
            resolution = settingsFile.GetProperty<int>("resolution");
            fullscreen = bool.Parse(settingsFile.GetProperty<string>("fullscreen"));
        }

        /// <summary>
        /// Saves the current settings to the Settings.cfg file
        /// </summary>
        private void SaveSettingsToFile()
        {
            Configuration settingsFile = FileManager.LoadConfig("Settings");
            
            //TODO: SAVE SETTINGS TO FILE

            //Camera settings
            //Save the cameraControl int to the "cameracontrol" setting in the file

            //Resolution setting
            //Save the resolution int to the "resolution" setting in the file

            //Fullscreen setting
            //Save the fullscreen bool to the "fullscreen" setting in the file
        }

        public void ApplySettings()
        {
            //Apply the camera settings
            MaxOfEmpires.camera.ControlMode = CameraControl;

            //Apply the fullscreen settings
            switch(Fullscreen)
            {
                case true:
                    {
                        //turn fullscreen on
                        MaxOfEmpires.fullscreen = true;
                        break;
                    }
                case false:
                default:
                    {
                        //turn fullscreen off
                        MaxOfEmpires.fullscreen  = false;
                        break;
                    }
            }

            //Apply the resolution settings
            switch (Resolution)
            {
                case 1:
                    {
                        //resolution = 800 * 480
                        MaxOfEmpires.ScreenSize = new Point(800, 480);
                        break;
                    }
                case 3:
                    {
                        //resolution = 1920 * 1080
                        MaxOfEmpires.ScreenSize = new Point(1920, 1080);
                        break;
                    }
                case 2:
                default:
                    {
                        //resolution = 1280 * 768
                        MaxOfEmpires.ScreenSize = new Point(1280, 768);
                        break;
                    }
            }

        }
       
        public void ResetSettings()
        {
            CameraControl = 3;
            Resolution = 2;
            fullscreen = false;
        }

        public void SaveCurrentSettings()
        {
            prevCameraControl = CameraControl;
            prevResolution = Resolution;
            prevFullscreen = Fullscreen;
        }

        public void BackToSaved()
        {
            CameraControl = prevCameraControl;
            Resolution = prevResolution;
            Fullscreen = prevFullscreen;
        }

        public int CameraControl
        {
            get
            {
                return cameraControl;
            }
            set
            {
                cameraControl = value;
            }
        }

        public int Resolution
        {
            get
            {
                return resolution;
            }

            set
            {
                resolution = value;
            }
        }

        public bool Fullscreen
        {
            get
            {
                return fullscreen;
            }
            set
            {
                fullscreen = value;
            }
        }
    }
}
