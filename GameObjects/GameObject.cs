using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.GameObjects
{
    /// <summary>
    /// Game objects of this game. Used to create things like units, buildings, lists of other objects, the battle grids, etc.
    /// </summary>
    abstract class GameObject
    {
        /// <summary>
        /// The internal name of this object.
        /// </summary>
        private string id;

        /// <summary>
        /// The parent object of this GameObject. Is most likely a GameObjectList
        /// </summary>
        /// <see cref="GameObjectList"/>
        private GameObject parent;

        /// <summary>
        /// The drawing position relative to the parent. 
        /// </summary>
        protected Vector2 position;

        /// <summary>
        /// Base constructor for new GameObjects. They can require to have some sort of id string.
        /// </summary>
        /// <param name="id">The internal name of the object.</param>
        protected GameObject(string id = "")
        {
            this.id = id;
        }

        /// <summary>
        /// Called when this object should be drawn. 
        /// </summary>
        /// <param name="time">The elapsed/current game time.</param>
        /// <param name="s">The spritebatch to draw this object with.</param>
        public abstract void Draw(GameTime time, SpriteBatch s);

        /// <summary>
        /// Handles input for this object.
        /// </summary>
        /// <param name="helper">The InputHelper. Use only for mouse input.</param>
        /// <param name="keyManager">The KeyManager. Use for keyboard(/controller?) input.</param>
        public virtual void HandleInput(InputHelper helper, KeyManager keyManager)
        {
        }

        /// <summary>
        /// Resets this object. Should be used for resetting animation, for instance.
        /// </summary>
        public virtual void Reset() // Most important for GameStates in our case; might want to remove this, if never used. 
        {
        }

        /// <summary>
        /// Called when a new turn should be calculated.
        /// </summary>
        /// <param name="turn">The current turn.</param>
        /// <param name="player">The player whose turn it is.</param>
        public virtual void TurnUpdate(uint turn, bool player)
        {
        }

        /// <summary>
        /// Called when this object should be updated. Should do things like animation.
        /// </summary>
        /// <param name="time">The current/elapsed game time.</param>
        public virtual void Update(GameTime time)
        {
        }

        public Vector2 DrawPosition
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.DrawPosition + position;
                }
                return position - MaxOfEmpires.camera.Position;
            }
            set
            {
                position = value;
            }
        }

        /// <summary>
        /// Returns the gameworld this object is in. Essentially returns the list at the top of the hierarchy.
        /// </summary>
        public GameObject GameWorld
        {
            get
            {
                if (Parent == null)
                    return this;
                return Parent.GameWorld;
            }
        }

        /// <summary>
        /// The internal name of this object.
        /// </summary>
        public string Id => id;

        /// <summary>
        /// Property for getting/setting this object's parent.
        /// </summary>
        public GameObject Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }
    }
}
