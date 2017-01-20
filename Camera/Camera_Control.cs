using Microsoft.Xna.Framework;
using MaxOfEmpires.GameStates;

namespace MaxOfEmpires
{
    public partial class Camera
    {

        /// <summary>
        /// The horizontal breakoff point for the mouse controlled camera
        /// </summary>
        private int cameraBreakoffX = 480;

        /// <summary>
        /// The vertical breakoff point for the mouse controlled camera
        /// </summary>
        private int cameraBreakoffY = GraphicsDeviceManager.DefaultBackBufferHeight;

        /// <summary>
        /// The margin in pixels from the breakoff points with which the mouse will move the camera
        /// </summary>
        private int cameraMouseMargin = 5;

        /// <summary>
        /// The speed with which the camera moves
        /// </summary>
        private float cameraMoveSpeed = 10.00f;

        /// <summary>
        /// A bool which, if true, will let the camera be controlled through both the mouse and the keyboard
        /// </summary>
        private bool useBoth = false;

        /// <summary>
        /// A bool determining if the camera should be controlled through the mouse
        /// True means mouse controll, False means keyboard controll
        /// </summary>
        private bool useMouse = false;

        /// <summary>
        /// The speed with which the camera zooms
        /// </summary>
        private float zoomSpeed = 0.02f;

        /// <summary>
        /// A function that checks if the mouse is in such a position that the camera should move.
        /// </summary>
        /// <param name="useMouse">A boolean that determines if the camera should be controlled through the mouse or keyboard.</param>
        /// <param name="inputHelper">The input helper.</param>
        public void CheckMousePositionForCamera(bool useMouse, bool useBoth, InputHelper inputHelper, KeyManager keyManager)
        {
            if (!useMouse || useBoth)
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

            if (useMouse || useBoth)
            {
                if (inputHelper.GetMousePosition(false).Y < CameraMouseMargin && inputHelper.GetMousePosition(false).Y >= 0)
                {
                    MoveCamera("up");
                }

                if (inputHelper.GetMousePosition(false).Y > CameraBreakoffY - CameraMouseMargin && inputHelper.GetMousePosition(false).Y<= CameraBreakoffY)
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

        }

        /// <summary>
        /// This function moves the camera in a specified direction.
        /// </summary>
        /// <param name="direction">The direction to which the camera should move (up, down, left, right).</param>
        public void MoveCamera(string direction)
        {
            double zoomCap = 15 / GameStateManager.GridSize.X;
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
            float cameraGridCompFactorX = ((GameStateManager.GridSize.X - 15) * 32);
            float cameraGridCompFactorY = ((GameStateManager.GridSize.Y - 15) * 32);
            float cameraGridCalc = 480 - (480 / Zoom);

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
            CheckMousePositionForCamera(UseMouse, UseBoth, inputHelper, keyManager);
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
        public bool UseMouse
        {
            get
            {
                return useMouse;
            }
            set
            {
                useMouse = value;
            }
        }

        /// <summary>
        /// A bool which, if true, will let the camera be controlled through both the mouse and the keyboard
        /// </summary>
        public bool UseBoth
        {
            get
            {
                return useBoth;
            }
            set
            {
                useBoth = value;
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
