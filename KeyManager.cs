using Microsoft.Xna.Framework;
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
        private Dictionary<Keys, string> keysByName;
        
        private KeyManager()
        {
            keyHandlers = new Dictionary<string, KeyPressHandler>();
            keysByName = new Dictionary<Keys, string>();
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
            if (keysByName.ContainsKey(key))
                throw new ArgumentException("Key '" + key.ToString() + "' was already registered. ");

            // Make sure the key's name was not yet registered
            if (keysByName.ContainsValue(name))
                throw new ArgumentException("Key name '" + name + "' was already registered. ");

            // Register this key-name pair
            keysByName[key] = name;
            keyHandlers[name] = handler;
        }

        public void RegisterKey(string name, Keys key)
        {
            keysByName.Add(key, name);
        }

        /// <summary>
        /// Checks for every registered key whether it has been pressed, and execute the corresponding handlers if they are.
        /// </summary>
        /// <param name="time">The time that has passed since last update.</param>
        /// <param name="helper">The <code>InputHelper</code> to check key presses with.</param>
        public void Update(GameTime time, InputHelper helper)
        {
            // Check for every registered key... 
            foreach (Keys k in keysByName.Keys)
            {
                // ... whether it has been pressed... 
                if (helper.KeyPressed(k))
                {
                    // ... and execute the corresponding handler
                    string keyName = keysByName[k];
                    KeyPressHandler handler = keyHandlers[keyName];
                    handler();
                }
            }
        }

        public static KeyManager Instance => manager;
    }
}
