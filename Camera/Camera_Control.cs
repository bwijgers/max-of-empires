using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MaxOfEmpires.GameStates;

namespace MaxOfEmpires
{
    public partial class Camera
    {
        #region Variables
        /// <summary>
        /// The horizontal breakoff point for the mouse controlled camera
        /// </summary>
        private int cameraBreakoffX = (int)MaxOfEmpires.OverlayPos.X;

        /// <summary>
        /// The vertical breakoff point for the mouse controlled camera
        /// </summary>
        private int cameraBreakoffY = MaxOfEmpires.ScreenSize.Y;

        /// <summary>
        /// The margin in pixels from the breakoff points with which the mouse will move the camera
        /// </summary>
        private int cameraMouseMargin = 15;

        /// <summary>
        /// The speed with which the camera moves
        /// </summary>
        private float cameraMoveSpeed = 2.00f;

        /// <summary>
        /// An int determining if the camera should be controlled through the mouse
        /// 1 means mouse control, 2 means keyboard control, 3 means both
        /// </summary>
        private int controlMode = MaxOfEmpires.settings.CameraControl;

        /// <summary>
        /// The speed with which the camera zooms
        /// </summary>
        private float zoomSpeed = 0.02f;

        /// <summary>
        /// The minimal value for the zoom
        /// </summary>
        private float zoomMin = 1.0f;

        /// <summary>
        /// The maximum value for the zoom
        /// </summary>
        private float zoomMax = 5.0f;
        #endregion

        /// <summary>
        /// A function that checks if the mouse is in such a position that the camera should move.
        /// </summary>
        /// <param name="useMouse">A boolean that determines if the camera should be controlled through the mouse or keyboard.</param>
        /// <param name="inputHelper">The input helper.</param>
        public void CheckMousePositionForCamera(int controlMode, InputHelper inputHelper, KeyManager keyManager)
        {
            switch (controlMode)
            {
                case 1:
                    {
                        MouseControlCheck(inputHelper, keyManager);
                        break;
                    }

                case 2:
                    {
                        KeyControlCheck(inputHelper, keyManager);
                        break;
                    }
                case 3:
                    {
                        MouseControlCheck(inputHelper, keyManager);
                        KeyControlCheck(inputHelper, keyManager);
                        break;
                    }
            }

        }

        private void MouseControlCheck(InputHelper inputHelper, KeyManager keyManager)
        {
            if (inputHelper.GetMousePosition(false).Y < CameraMouseMargin && inputHelper.GetMousePosition(false).Y >= 0)
            {
                MoveCamera("up");
            }

            if (inputHelper.GetMousePosition(false).Y > CameraBreakoffY - CameraMouseMargin && inputHelper.GetMousePosition(false).Y <= CameraBreakoffY)
            {
                MoveCamera("down");
            }

            if (inputHelper.GetMousePosition(false).X < CameraMouseMargin && inputHelper.GetMousePosition(false).X >= 0)
            {
                MoveCamera("left");
            }

            if (inputHelper.GetMousePosition(false).X > CameraBreakoffX - CameraMouseMargin && inputHelper.GetMousePosition(false).X <= CameraBreakoffX)
            {
                MoveCamera("right");
            }

            if (inputHelper.MouseScrollUp)
            {
                MoveCamera("in");
            }

            if (inputHelper.MouseScrollDown)
            {
                MoveCamera("out");
            }
        }

        private void KeyControlCheck(InputHelper inputHelper, KeyManager keyManager)
        {
            if (keyManager.IsKeyDown("moveCameraUp", inputHelper))
            {
                MoveCamera("up");
            }

            if (keyManager.IsKeyDown("moveCameraDown", inputHelper))
            {
                MoveCamera("down");
            }

            if (keyManager.IsKeyDown("moveCameraRight", inputHelper))
            {
                MoveCamera("right");
            }

            if (keyManager.IsKeyDown("moveCameraLeft", inputHelper))
            {
                MoveCamera("left");
            }

            if (keyManager.IsKeyDown("zoomCameraIn", inputHelper))
            {
                MoveCamera("in");
            }

            if (keyManager.IsKeyDown("zoomCameraOut", inputHelper))
            {
                MoveCamera("out");
            }
        }

        /// <summary>
        /// This function moves the camera in a specified direction.
        /// </summary>
        /// <param name="direction">The direction to which the camera should move (up, down, left, right).</param>
        public void MoveCamera(string direction)
        {
            double zoomCap = ((double)MaxOfEmpires.ScreenSize.Y/32)/ (GameStateManager.GridSize.X);
            switch (direction)
            {
                // Moves the camera upwards
                case "up":
                case "Up":
                    { 
                        MaxOfEmpires.camera.Position += new Vector2(0, -CameraMoveSpeed);
                        break;
                    }

                // Moves the camera downwards
                case "down":
                case "Down":
                    {
                        MaxOfEmpires.camera.Position += new Vector2(0, CameraMoveSpeed);
                        break;
                    }

                // Moves the camera towards the left
                case "left":
                case "Left":
                    {
                        MaxOfEmpires.camera.Position += new Vector2(-CameraMoveSpeed, 0);
                        break;
                    }
                    
                // Moves the camera towards the right
                case "right":
                case "Right":
                    {
                        MaxOfEmpires.camera.Position += new Vector2(CameraMoveSpeed, 0);
                        break;
                    }

                // Zooms the camera in
                case "in":
                case "In":
                    {
                        Zoom = MathHelper.Clamp(Zoom + ZoomSpeed, (float)zoomCap, 5.0f);
                        break;
                    }

                // Zooms the camera out
                case "out":
                case "Out":
                    {
                        Zoom = MathHelper.Clamp(Zoom - ZoomSpeed, (float)zoomCap, 5.0f);
                        break;
                    }
            }

            // Calculations needed to cap the camera
            float cameraGridCompFactorX = 0;
            float cameraGridCompFactorY = 0;
            float cameraGridCalc = 0;
            switch (MaxOfEmpires.settings.Resolution)
            {
                //800 x 480
                case 1:
                    {
                        cameraGridCompFactorX = ((GameStateManager.GridSize.X - 15) * 32);
                        cameraGridCompFactorY = ((GameStateManager.GridSize.Y - 15) * 32);
                        cameraGridCalc = 480 - (480 / Zoom);
                        break;
                    }
                //1280 x 768
                case 2:
                    {
                        cameraGridCompFactorX = ((GameStateManager.GridSize.X - 24) * 32);
                        cameraGridCompFactorY = ((GameStateManager.GridSize.Y - 24) * 32);
                        cameraGridCalc = 768 - (768 / Zoom);
                        break;
                    }
                //1920 x 1080
                case 3:
                    {
                        cameraGridCompFactorX = ((GameStateManager.GridSize.X - 33.75f) * 32);
                        cameraGridCompFactorY = ((GameStateManager.GridSize.Y - 33.75f) * 32);
                        cameraGridCalc = 1080 - (1080 / Zoom);
                        break;
                    }
            }
            
            // Caps the camera so it wont move past the grid
            float x = MathHelper.Clamp(MaxOfEmpires.camera.Position.X, 0.0f, (int)(cameraGridCalc + cameraGridCompFactorX));
            float y = MathHelper.Clamp(MaxOfEmpires.camera.Position.Y, 0.0f, (int)(cameraGridCalc + cameraGridCompFactorY));
            MaxOfEmpires.camera.Position = new Vector2(x, y);
        }

        /// <summary>
        /// The update of the camera.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="inputHelper">The inputhelper.</param>
        public void Update(GameTime gameTime, InputHelper inputHelper, KeyManager keyManager)
        {
            CheckMousePositionForCamera(ControlMode, inputHelper, keyManager);
        }

        /// <summary>
        /// Gets or sets the horizontal breakoff point for the mouse controlled camera.
        /// </summary>
        public int CameraBreakoffX
        {
            get
            {
                return cameraBreakoffX;
            }
            set
            {
                cameraBreakoffX = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical breakoff point for the mouse controlled camera.
        /// </summary>
        public int CameraBreakoffY
        {
            get
            {
                return cameraBreakoffY;
            }
            set
            {
                cameraBreakoffY = value;
            }
        }

        /// <summary>
        /// Gets or sets the margin in pixels from the breakoff points with which the mouse will move the camera.
        /// </summary>
        public int CameraMouseMargin
        {
            get
            {
                return cameraMouseMargin;
            }
            set
            {
                cameraMouseMargin = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed with which the camera moves.
        /// </summary>
        public float CameraMoveSpeed
        {
            get
            {
                return cameraMoveSpeed/zoom;
            }
            set
            {
                cameraMoveSpeed = value;
            }
        }

        /// <summary>
        /// A bool determining if the camera should be controlled through the mouse
        /// True means mouse controll, False means keyboard controll
        /// </summary>
        public int ControlMode
        {
            get
            {
                return controlMode;
            }
            set
            {
                controlMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed with which the camera zooms.
        /// </summary>
        public float ZoomSpeed
        {
            get
            {
                return zoomSpeed*zoom;
            }
            set
            {
                zoomSpeed = value;
            }
        }

    }
}
