using Microsoft.Xna.Framework;

namespace MaxOfEmpires
{
    public partial class Camera
    {
        private Vector2 position;

        /// <summary>
        /// initialises the camera at position 0, 0
        /// </summary>
        public Camera()
        {
            position = new Vector2(0, 0);
        }

        /// <summary>
        /// returns or sets the camera position
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
            }
        }

        public void Reset()
        {
            Position = new Vector2(0, 0);
        }

        /// <summary>
        /// returns or sets the camera center
        /// </summary>
        public Vector2 Center
        {
            get { return new Vector2(Position.X + 1920 / 2, Position.Y + 1080 / 2); }
            set
            {
                position.X = value.X - 1920 / 2;
                position.Y = value.Y - 1080 / 2;
            }
        }

    }
}
