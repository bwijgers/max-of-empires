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

namespace Ebilkill.Gui.Elements
{
    class GuiList : GuiElement
    {
        private GuiButton up;
        private GuiButton down;
        private List<GuiElement> allElements;
        private int currentTop;
        private int displayableItemsCount;
        private int itemHeight;
        private int itemWidth;
        private int maxWidth;

        public static GuiList createNewList(Point position, int displayableItems, List<GuiElement> elements, int maxWidth = -1)
        {
            // Create a container for the width of the display
            int width = maxWidth;

            // If the max width is bogus, calculate the width
            if(width < 16)
            {
                width = 16;

                // Get the biggest width of labels
                foreach (GuiElement element in elements)
                {
                    width = Math.Max(element.Bounds.Width + 16, width);
                }
            }

            // Get the height of the list.
            int height = 0;
            try
            {
                height = elements[0].Bounds.Height * displayableItems;
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            height = Math.Max(height, 34);

            // Return a new GuiList based on position, width and height. 
            GuiList retVal = new GuiList(new Rectangle(position, new Point(width, height)), elements);
            retVal.displayableItemsCount = displayableItems;
            retVal.itemHeight = height / displayableItems;
            retVal.itemWidth = width - 16;
            retVal.calculateElementPositions();
            return retVal;
        }

        private GuiList(Rectangle bounds, List<GuiElement> elements) : base(bounds)
        {
            // Create the up-button
            up = new GuiButton(new Rectangle(new Point(0, 0), new Point(16)), "ArrowUp");
            up.ClickHandler = () => scroll(true);
            up.loadContent(AssetManager.Instance);

            // Create the down-button
            down = new GuiButton(new Rectangle(new Point(0, 0), new Point(16)), "ArrowDown");
            down.ClickHandler = () => scroll(false);
            down.loadContent(AssetManager.Instance);

            // Create the list-related variables
            allElements = elements;
            currentTop = 0;

            // Make sure all labels know this is their parent
            foreach (GuiElement element in allElements)
            {
                element.Parent = this;
            }

            // Make sure the buttons know this is their parent
            up.Parent = this;
            down.Parent = this;

            maxWidth = bounds.Width;
        }

        private bool recalculateBounds()
        {
            if (allElements.Count <= 0)
            {
                Visible = false;
                return false;
            }

            // Buttons should be visible only when we have to scroll...
            up.Visible = down.Visible = allElements.Count > displayableItemsCount;

            itemHeight = allElements[0].Bounds.Height;
            int height = Math.Max(34, itemHeight * Math.Min(allElements.Count, displayableItemsCount));
            int width = maxWidth;

            Bounds = new Rectangle(Bounds.Location, new Point(width, height));

            // We're visible, this should be fine
            return true;
        }

        public void calculateElementPositions()
        {
            // If we're not visible, just don't do anything at all.
            if (!recalculateBounds())
            {
                return;
            }

            // Assume we're visible
            bool visible = true;

            // Set every label's position
            for (int i = 0; i < allElements.Count; ++i)
            {
                // Set the location of the label to its position in the list
                allElements[i].move(new Point(Bounds.Location.X, Bounds.Location.Y - (currentTop - i) * itemHeight));
                if (!allElements[i].Visible)
                    visible = false;
            }
            Visible = visible;

            up.move(new Point(Bounds.Right - 16, Bounds.Top));
            down.move(new Point(Bounds.Right - 16, Bounds.Bottom - 16));
        }

        public void clear()
        {
            this.allElements.Clear();
            calculateElementPositions();
        }

        private void scroll(bool up, bool allTheWay = false)
        {
            int move = up ? -1 : 1;

            if (currentTop + move < 0 || currentTop + move + displayableItemsCount > allElements.Count)
            {
                return;
            }

            if (allTheWay)
            {
                while (currentTop + move < 0 || currentTop + move + displayableItemsCount > allElements.Count)
                {
                    currentTop += move;
                }
            }

            currentTop += move;
            calculateElementPositions();
        }

        public override void drawElement(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            for (int i = currentTop; i < currentTop + displayableItemsCount && i < allElements.Count; ++i)
            {
                allElements[i].drawElement(spriteBatch);
            }

            up.drawElement(spriteBatch);
            down.drawElement(spriteBatch);
        }

        public override void loadContent(AssetManager content)
        {
            up.loadContent(content);
            down.loadContent(content);
            foreach (GuiElement element in allElements)
                element.loadContent(content);
        }

        public override void onClick(ClickEvent e)
        {
            if (up.Bounds.Contains(e.Position))
                up.onClick(e);

            if (down.Bounds.Contains(e.Position))
                down.onClick(e);

            foreach (GuiElement element in allElements)
            {
                if (element.Bounds.Contains(e.Position))
                {
                    element.onClick(e);
                }
            }
        }

        public override void onVisibilityChange(bool newVisibility)
        {
            if (newVisibility)
            {
                scroll(true, true);
            }
        }

        public void addElement(GuiElement label, int index = -1)
        {
            // Add the label, possibly at a specified index
            if (index < 0 || index >= allElements.Count)
                allElements.Add(label);
            else
                allElements.Insert(index, label);

            // Make sure all labels are in the correct positions
            calculateElementPositions();
        }

        public void removeElement(int index)
        {
            // Only remove the label if it's in a valid position
            if (index > 0 && index < allElements.Count)
            {
                allElements.RemoveAt(index);
            }
        }

        // TODO: see if this works as intended
        public void removeLabel(GuiLabel label)
        {
            int index = allElements.FindIndex(gl => label.Equals(gl));
            removeElement(index);
        }

        public List<GuiElement> AllLabels => allElements;

        public int MaxHeight => itemHeight * displayableItemsCount;
    }
}
