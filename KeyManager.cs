using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MaxOfEmpires
{
    public class KeyManager
    {
        private static KeyManager manager = new KeyManager();
        
        /// <summary>
        /// A delegate for pressing a key registered by the KeyManager.
        /// </summary>
        public delegate void KeyPressHandler();

        private Dictionary<string, KeyPressHandler> keyHandlers;
        private Dictionary<string, Keys> keysByName;
        
        private KeyManager()
        {
            keyHandlers = new Dictionary<string, KeyPressHandler>();
            keysByName = new Dictionary<string, Keys>();
        }

        /// <summary>
        /// Checks whether the a certain key on the keyboard is currently down.
        /// </summary>
        /// <param name="name">The name of the key to check for.</param>
        /// <param name="helper">The InputHelper to use for keyboard input.</param>
        /// <returns>True if the specified key is down, false otherwise.</returns>
        /// <exception cref="ArgumentException">When the specified name is not found.</exception>
        public bool IsKeyDown(string name, InputHelper helper)
        {
            // Make sure the name supplied is registered.
            if (!keysByName.ContainsKey(name))
            {
                throw new ArgumentException("Key '" + name + "' was not registered but was asked for.");
            }

            // Return whether the specified key is down.
            return helper.IsKeyDown(keysByName[name]);
        }

        /// <summary>
        /// Checks whether the a certain key on the keyboard was pressed during last update.
        /// </summary>
        /// <param name="name">The name of the key to check for.</param>
        /// <param name="helper">The InputHelper to use for keyboard input.</param>
        /// <returns>True if the specified key was pressed, false otherwise.</returns>
        /// <exception cref="ArgumentException">When the specified name is not found.</exception>
        public bool KeyPressed(string name, InputHelper helper)
        {
            // Make sure the name supplied is registered
            if(!keysByName.ContainsKey(name))
            {
                throw new ArgumentException("Key '" + name + "' was not registered but was requested.");
            }

            // Return whether the specified key was pressed this update.
            return helper.KeyPressed(keysByName[name]);
        }

        /// <summary>
        /// Registers a key to a name and a handler.
        /// </summary>
        /// <param name="name">The name to register the key by.</param>
        /// <param name="key">The key to register.</param>
        /// <param name="handler">The handler called when this button is pressed.</param>
        public void RegisterKey(string name, Keys key, KeyPressHandler handler)
        {
            if (name == null || key == Keys.None || handler == null)
            {
                throw new ArgumentException("Arguments 'name' and 'handler' can't be null, and argument 'key' can't be Keys.None.");
            }
             
            // Make sure the key was not yet registered
            if (keysByName.ContainsKey(name))
                throw new ArgumentException("Key '" + key.ToString() + "' was already registered. ");

            // Make sure the key's name was not yet registered
            if (keysByName.ContainsValue(key))
                throw new ArgumentException("Key name '" + name + "' was already registered. ");

            // Register this key-name pair
            keysByName[name] = key;
            keyHandlers[name] = handler;
        }

        /// <summary>
        /// Registers a key with a name, but without a handler.
        /// </summary>
        /// <param name="name">The name to register the key under.</param>
        /// <param name="key">The key to register.</param>
        public void RegisterKey(string name, Keys key)
        {
            keysByName.Add(name, key);
        }

        public static KeyManager Instance => manager;
    }
}
