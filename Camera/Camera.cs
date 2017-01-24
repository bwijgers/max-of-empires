using MaxOfEmpires.GameStates;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires
{
    public partial class Camera
    {
        //Variables

        /// <summary>
        /// The Vector2 determining the position of the camera
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// A float determining the zoom value of the camera
        /// </summary>
        private float zoom;


        // Functions

        /// <summary>
        /// initialises the camera at position 0, 0
        /// </summary>
        public Camera()
        {
            Position = new Vector2(0, 0);
            Zoom = 1.0f;
            switch (MaxOfEmpires.settings.Resolution)
            {
                //800 x 480
                case 1:
                    {
                        cameraBreakoffY = 480;
                        zoomMin = 15 / GameStateManager.GridSize.X;
                        break;
                    }
                //1280 x 768
                case 2:
                    {
                        cameraBreakoffY = 768;
                        zoomMin = 24 / GameStateManager.GridSize.X;
                        break;
                    }
                //1920 x 1080
                case 3:
                    {
                        cameraBreakoffY = 1080;
                        zoomMin = 33.75f / GameStateManager.GridSize.X;
                        break;
                    }
            }
        }

        /// <summary>
        /// Resets the camera to (0, 0)
        /// </summary>
        public void Reset()
        {
            Position = new Vector2(0, 0);
            zoom = 1.0f;
        }

		// Properties

        /// <summary>
        /// Gets or sets the camera center
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2(Position.X + 1920 / 2, Position.Y + 1080 / 2);
            }
            set
            {
                position.X = value.X - 1920 / 2;
                position.Y = value.Y - 1080 / 2;
            }
        }

        /// <summary>
        /// Gets or sets the camera position
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the zoom of the camera
        /// </summary>
        public float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = value;
            }
        }
    }
}
