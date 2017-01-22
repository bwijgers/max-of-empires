using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires
{
    public class InputHelper
    {
        private Dictionary<Keys, char> alphabetKeys;
        private KeyboardState currentKeyboard, previousKeyboard;
        private MouseState currentMouse, previousMouse;
        private Vector2 displayScale;

        /// <summary>
        /// Creates a new input helper.
        /// </summary>
        public InputHelper()
        {
            alphabetKeys = new Dictionary<Keys, char>();
            currentKeyboard = previousKeyboard = Keyboard.GetState();
            currentMouse = previousMouse = Mouse.GetState();
            displayScale = Vector2.One;
        }

        public Vector2 GetMousePosition(bool basedOnCamera)
        {
            if (!basedOnCamera)
            {
                return currentMouse.Position.ToVector2() * displayScale;
            }

            else
            {
                return currentMouse.Position.ToVector2() / MaxOfEmpires.camera.Zoom * displayScale;
            }
        }

        /// <summary>
        /// Checks whether a key on the keyboard is currently held down.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if the key is currently down, false otherwise.</returns>
        public bool IsKeyDown(Keys k)
        {
            return currentKeyboard.IsKeyDown(k);
        }

        /// <summary>
        /// Checks whether a key on the keyboard was pressed during the last update.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <param name="v"></param>
        /// <returns>True if the key was pressed last frame, false otherwise.</returns>
        public bool KeyPressed(Keys k, bool v = true)
        {
            return currentKeyboard.IsKeyDown(k) && previousKeyboard.IsKeyUp(k);
        }

        /// <summary>
        /// Updates this InputHelper. Updates both mouse and keyboard state.
        /// </summary>
        /// <param name="time">The current/elapsed game time.</param>
        public void Update(GameTime time)
        {
            // Update mouse input
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();

            // Update keyboard input
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();
        }

        public Vector2 DisplayScale
        {
            get
            {
                return displayScale;
            }
            set
            {
                displayScale = value;
            }
        }

        public bool MouseLeftButtonPressed => currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released;
        public bool MouseRightButtonPressed => currentMouse.RightButton == ButtonState.Pressed && previousMouse.RightButton == ButtonState.Released;
        public bool MouseScrollDown => currentMouse.ScrollWheelValue < previousMouse.ScrollWheelValue;
        public bool MouseScrollUp => currentMouse.ScrollWheelValue > previousMouse.ScrollWheelValue;

        public Dictionary<Keys, char> TextKeys => alphabetKeys;
    }
}
 