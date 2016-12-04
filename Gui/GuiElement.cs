using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui.Events;
using MaxOfEmpires;

namespace Ebilkill.Gui
{
    public abstract class GuiElement
    {
        private Rectangle bounds;
        protected bool visible;
        private GuiElement parent;

        protected GuiElement(Rectangle bounds)
        {
            this.bounds = bounds;
            visible = true;
            parent = null;
        }

        /// <summary>
        /// Called when the content of this GuiElement should be loaded.
        /// </summary>
        /// <param name="content">The AssetManager to load the content with.</param>
        public abstract void loadContent(AssetManager content);

        /// <summary>
        /// Called by the parent GuiScreen when this GuiElement should be drawn to the screen.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw the element with.</param>
        /// <param name="graphics">The GraphicsDevice used by the game window.</param>
        public abstract void drawElement(SpriteBatch spriteBatch);

        public virtual void onClick(ClickEvent @event)
        {
        }

        public virtual void handleKeyboardInput(InputHelper helper)
        {
        }

        public virtual void onVisibilityChange(bool newVisibility)
        {
        }

        /// <summary>
        /// Repositions this GuiElement to another place.
        /// </summary>
        /// <param name="pos">The position to move to.</param>
        public void move(Point pos)
        {
            bounds.Location = pos;
        }

        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            protected set
            {
                bounds = value;
            }
        }

        public virtual bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    onVisibilityChange(value);
                    visible = value;
                }
            }
        }

        public GuiElement Parent
        {
            set
            {
                this.parent = value;
            }
            protected get
            {
                return parent;
            }
        }
    }
}
