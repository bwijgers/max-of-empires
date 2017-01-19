﻿using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MaxOfEmpires
{
    public class AssetManager
    {
        private static AssetManager instance;

        public static void Init(ContentManager content)
        {
            instance = new AssetManager(content);
        }

        public static AssetManager Instance => instance;

        private Dictionary<string, object> assetDict;
        private ContentManager content;

        private AssetManager(ContentManager content)
        {
            assetDict = new Dictionary<string, object>();
            this.content = content;
        }

        /// <summary>
        /// Get an asset from this <code>AssetManager</code>. Load it if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type of the asset to load.</typeparam>
        /// <param name="name">The name of the asset to load.</param>
        /// <returns>The asset to load.</returns>
        public T getAsset<T>(string name) where T : class
        {
            // Check if the asset exists
            if (assetDict.ContainsKey(name))
            {
                object s = assetDict[name];
                if (s is T)
                    return (T)s;

                // The asset exists, but it is not of this type. This must be an error.
                throw new ArgumentException("Asset " + name + " was loaded before, but is not of this type.");
            }

            // The asset does not yet exist. Let's make it so
            if (typeof(T).Equals(typeof(Spritesheet)))
            {
                return (T)LoadSpritesheet(name);
            }
            else if (typeof(T).Equals(typeof(Animation)))
            {
                return (T)LoadAnimation(name);
            }

            assetDict[name] = content.Load<T>(name);
            return (T)assetDict[name];
        }

        private object LoadAnimation(string name)
        {
            Spritesheet sheet = (Spritesheet)LoadSpritesheet(name);
        }

        private object LoadSpritesheet(string name)
        {
            Texture2D tex = content.Load<Texture2D>(name);
            int width, height;
            try
            {
                string size = name.Split('@')[1];
                width = int.Parse(size.Split('x')[0]);
                height = int.Parse(size.Split('x')[1]);
            }
            catch (IndexOutOfRangeException e)
            {
                width = height = 1;
            }
            catch (FormatException e)
            {
                width = height = 1;
            }
            Spritesheet sheet = new Spritesheet(tex, width, height);
            assetDict[name] = sheet;
            return assetDict[name];
        }

        /// <summary>
        /// Alternative to <code>T getAsset&lt;T&gt;</code>, but less type safe.
        /// </summary>
        /// <param name="s">The name of the asset to load</param>
        /// <returns>An <code>object</code> representing the asset.</returns>
        public object this[string s] => assetDict[s];
    }
}
