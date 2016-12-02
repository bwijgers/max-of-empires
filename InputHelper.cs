﻿using Microsoft.Xna.Framework;
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
        private MouseState currentMouse, previousMouse;
        private KeyboardState currentKeyboard, previousKeyboard;

        public InputHelper()
        {
            alphabetKeys = new Dictionary<Keys, char>();
            currentKeyboard = previousKeyboard = Keyboard.GetState();
            currentMouse = previousMouse = Mouse.GetState();
        }

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

        public Dictionary<Keys, char> AlphabetKeys => alphabetKeys;
        public bool MouseLeftButtonPressed => currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released;
        public Vector2 MousePosition => currentMouse.Position.ToVector2();
        public bool MouseRightButtonPressed => currentMouse.RightButton == ButtonState.Pressed && previousMouse.RightButton == ButtonState.Released;
    }
}
