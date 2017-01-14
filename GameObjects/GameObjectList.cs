using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameObjects
{
    /// <summary>
    /// A list of GameObjects. This class is meant to group different GameObjects together if they don't belong in a specific orientation or order.
    /// </summary>
    class GameObjectList : GameObject
    {
        /// <summary>
        /// A function that can be called on every element in this list. Used in conjunction with the ForEach method.
        /// </summary>
        /// <param name="obj">The current GameObject in the iteration of all elements of this list.</param>
        /// <see cref="ForEach(ListFunc)"/>
        public delegate void ListFunc(GameObject obj);

        /// <summary>
        /// The list of all GameObjects contained by this list.
        /// </summary>
        private List<GameObject> children;

        /// <summary>
        /// Creates a new GameObjectList.
        /// </summary>
        public GameObjectList(string id = "") : base(id)
        {
            children = new List<GameObject>();
        }

        /// <summary>
        /// Adds a GameObject to this GameObjectList. Also sets the GameObject's parent equal to this list.
        /// </summary>
        /// <param name="obj">The GameObject to add to this list.</param>
        public void Add(GameObject obj)
        {
            children.Add(obj);
            obj.Parent = this;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            // Drawing a list just means drawing its children
            ForEach(child => child?.Draw(time, s));
        }

        /// <summary>
        /// Executes a specified void function for each GameObject in this GameObjectList.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        public void ForEach(ListFunc func)
        {
            for (int i = children.Count - 1; i >= 0; --i)
            {
                func.Invoke(children[i]);
            }
        }

        public override void HandleInput(InputHelper helper, KeyManager keyManager)
        {
            // Managing a list's input is pretty easy; just let everyone else handle theirs.
            ForEach(child => child?.HandleInput(helper, keyManager));
        }

        /// <summary>
        /// Removes a GameObject from this list. Does not search inner lists to remove the object from them; only elements contained in THIS list can be removed this way.
        /// </summary>
        /// <param name="obj">The GameObject to remove from this list.</param>
        public void RemoveChild(GameObject obj)
        {
            // Check if this list has the object
            if (children.Contains(obj))
            {
                // Remove it
                children.Remove(obj);
            }
        }

        public override void Reset()
        {
            // Resetting this list means resetting all of its kids
            ForEach(child => child?.Reset());
        }

        /// <see cref="GameObject.TurnUpdate(uint, bool)"/>
        public override void TurnUpdate(uint turn, Player player)
        {
            // Just TurnUpdate everything in the grid
            ForEach(child => child?.TurnUpdate(turn, player));
        }

        public override void Update(GameTime time)
        {
            // Updating this list means just update all children.
            ForEach(child => child?.Update(time));
        }

        /// <summary>
        /// The amount of children this GameObjectList currently has. 
        /// </summary>
        protected int Count => children.Count;
    }
}
