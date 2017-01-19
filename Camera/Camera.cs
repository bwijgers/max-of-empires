using Microsoft.Xna.Framework;

namespace MaxOfEmpires
{
    public partial class Camera
    {
        // Variables

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
        }

        /// <summary>
        /// Resets the camera to (0, 0)
        /// </summary>
        public void Reset()
        {
            Position = new Vector2(0, 0);
        }


        // Properties

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

    }
}
