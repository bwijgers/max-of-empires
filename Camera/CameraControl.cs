using Microsoft.Xna.Framework;

namespace MaxOfEmpires
{
    public partial class Camera
    {
        private float CameraMoveSpeed = 1.00f;
        private int CameraBreakoffX = GraphicsDeviceManager.DefaultBackBufferWidth;
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
                if (inputHelper.MousePosition.Y < CameraMouseMargin && inputHelper.MousePosition.Y >= 0)
                {
                    MoveCamera("up");
                }

                if (inputHelper.MousePosition.Y > CameraBreakoffY - CameraMouseMargin && inputHelper.MousePosition.Y<= CameraBreakoffY)
                {
                    MoveCamera("Down");
                }

                if (inputHelper.MousePosition.X < CameraMouseMargin && inputHelper.MousePosition.X >= 0)
                {
                    MoveCamera("Left");
                }

                if (inputHelper.MousePosition.X > CameraBreakoffX - CameraMouseMargin && inputHelper.MousePosition.X <= CameraBreakoffX)
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

                //moves the camera downwards
                case "left":
                case "Left":
                    {
                        MaxOfEmpires.camera.Position += new Vector2(-CameraMoveSpeed, 0);
                        break;
                    }
                    
                    //moves the camera downwards
                case "right":
                case "Right":
                    {
                        MaxOfEmpires.camera.Position += new Vector2(CameraMoveSpeed, 0);
                        break;
                    }

            }
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
