using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MaxOfEmpires;

namespace Ebilkill.Gui
{
    public abstract class GuiScreen
    {
        private List<GuiElement> elements;
        private int activeElement;

        protected GuiScreen()
        {
            elements = new List<GuiElement>();
            activeElement = -1;
        }

        public void addElement(GuiElement element)
        {
            elements.Add(element);
        }

        public void removeElement(GuiElement element)
        {
            elements.Remove(element);
        }

        public void clearElements()
        {
            elements.Clear();
        }

        public virtual void loadContent(AssetManager content)
        {
            foreach(GuiElement element in elements)
            {
                element.loadContent(content);
            }
        }

        public virtual void draw(SpriteBatch spriteBatch)
        {
            // Draw all elements
            foreach (GuiElement element in elements)
            {
                element.drawElement(spriteBatch);
            }
        }

        public virtual void update(InputHelper inputHelper)
        {
            if (inputHelper.MouseLeftButtonPressed)
            {
                // Set no element active if none has been pressed
                activeElement = -1;

                // Check if an element has been clicked.
                for (int i = 0; i < elements.Count; ++i)
                {
                    GuiElement element = elements[i];

                    // Only visible elements have events associated with them
                    if (!element.Visible)
                        continue;

                    // Check if this is a clicked element
                    if (element.Bounds.Contains(inputHelper.MousePosition)) {
                        // Handle the click
                        element.onClick(new Events.ClickEvent(inputHelper.MousePosition));

                        // Make this element active
                        activeElement = i;
                    }
                }
            }

            // Check if there is an active element and if it's visible
            GuiElement active = ActiveElement;
            if(active != null && active.Visible)
                active.handleKeyboardInput(inputHelper);
        }

        private GuiElement ActiveElement
        {
            get
            {
                // If there is an active element, return it
                if (activeElement >= 0 && activeElement < elements.Count)
                    return elements[activeElement];

                // Return null if there is no such element
                return null;
            }
        }
    }
}
