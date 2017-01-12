using Microsoft.Xna.Framework;
using MaxOfEmpires.GameStates;

namespace MaxOfEmpires
{
    public partial class Camera
    {
        private float CameraMoveSpeed = 1.00f;
        private int CameraBreakoffX = 480;
        private int CameraBreakoffY = GraphicsDeviceManager.DefaultBackBufferHeight;
        private int CameraMouseMargin = 5;
        public bool UseMouse = false;

        /// <summary>
        /// A function that checks if the mouse is in such a position that the camera should move.
        /// </summary>
        /// <param name="useMouse"> A boolean that determines if the camera should be controlled through the mouse or keyboard</param>
        /// <param name="inputHelper">the input helper</param>
        public void CheckMousePositionForCamera(bool useMouse, InputHelper inputHelper, KeyManager keyManager)
        {

            if (keyManager.IsKeyDown("zoomCameraIn", inputHelper))
            {
                MoveCamera("in");
            }

            if (keyManager.IsKeyDown("zoomCameraOut", inputHelper))
            {
                MoveCamera("out");
            }

            if (!useMouse)
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
            }

            if (useMouse)
            {
                if (inputHelper.GetMousePosition(false).Y < CameraMouseMargin && inputHelper.GetMousePosition(false).Y >= 0)
                {
                    MoveCamera("up");
                }

                if (inputHelper.GetMousePosition(false).Y > CameraBreakoffY - CameraMouseMargin && inputHelper.GetMousePosition(false).Y<= CameraBreakoffY)
                {
                    MoveCamera("Down");
                }

                if (inputHelper.GetMousePosition(false).X < CameraMouseMargin && inputHelper.GetMousePosition(false).X >= 0)
                {
                    MoveCamera("Left");
                }

                if (inputHelper.GetMousePosition(false).X > CameraBreakoffX - CameraMouseMargin && inputHelper.GetMousePosition(false).X <= CameraBreakoffX)
                {
                    MoveCamera("Right");
                }


            }

        }

        /// <summary>
        /// This function moves the camera in a specified direction.
        /// </summary>
        /// <param name="Direction"> The direction to which the camera should move (up, down, left, right) </param>
        public void MoveCamera(string Direction)
        {
            switch (Direction)
            {
                //Moves the camera upwards
                case "up":
                case "Up":
                    { 
                        MaxOfEmpires.camera.Position += new Vector2(0, -CameraMoveSpeed);
                        break;
                    }

                //moves the camera downwards
                case "down":
                case "Down":
                    {
                        MaxOfEmpires.camera.Position += new Vector2(0, CameraMoveSpeed);
                        break;
                    }

                //moves the camera towards the left
                case "left":
                case "Left":
                    {
                        MaxOfEmpires.camera.Position += new Vector2(-CameraMoveSpeed, 0);
                        break;
                    }
                    
                //moves the camera towards the right
                case "right":
                case "Right":
                    {
                        MaxOfEmpires.camera.Position += new Vector2(CameraMoveSpeed, 0);
                        break;
                    }

                //zooms the camera in
                case "in":
                case "In":
                    {
                        MaxOfEmpires.Zoom = MathHelper.Clamp(MaxOfEmpires.Zoom + 0.01f, 1.0f, 5.0f);
                        break;
                    }

                //zooms the camera out
                case "out":
                case "Out":
                    {
                        MaxOfEmpires.Zoom = MathHelper.Clamp(MaxOfEmpires.Zoom - 0.01f, 1.0f, 5.0f);

                        float cameraGridCompFactorX2 = ((GameStateManager.GridSize.X - 15) * 32);
                        float cameraGridCompFactorY2 = ((GameStateManager.GridSize.Y - 15) * 32);
                        float cameraGridCalc2 = 480 - (480 / MaxOfEmpires.Zoom);

                        //caps the camera so it wont move past the grid
                        float x2 = MathHelper.Clamp(MaxOfEmpires.camera.Position.X, 0.0f, (int)(cameraGridCalc2 + cameraGridCompFactorX2));
                        float y2 = MathHelper.Clamp(MaxOfEmpires.camera.Position.Y, 0.0f, (int)(cameraGridCalc2 + cameraGridCompFactorY2));
                        MaxOfEmpires.camera.Position = new Vector2(x2, y2);
                        break;
                    }
                

            }
            
            float cameraGridCompFactorX = ((GameStateManager.GridSize.X - 15) * 32);
            float cameraGridCompFactorY = ((GameStateManager.GridSize.Y - 15) * 32);
            float cameraGridCalc = 480 - (480 / MaxOfEmpires.Zoom);

            //caps the camera so it wont move past the grid
            float x = MathHelper.Clamp(MaxOfEmpires.camera.Position.X, 0.0f, (int)(cameraGridCalc + cameraGridCompFactorX));
            float y = MathHelper.Clamp(MaxOfEmpires.camera.Position.Y, 0.0f, (int)(cameraGridCalc + cameraGridCompFactorY));
            MaxOfEmpires.camera.Position = new Vector2(x, y);
        }

        /// <summary>
        /// The update of the camera.
        /// </summary>
        /// <param name="gameTime"> the current game time</param>
        /// <param name="inputHelper"> the inputhelper</param>
        public void Update(GameTime gameTime, InputHelper inputHelper, KeyManager keyManager)
        {
            CheckMousePositionForCamera(UseMouse, inputHelper, keyManager);
        }

    }
}
