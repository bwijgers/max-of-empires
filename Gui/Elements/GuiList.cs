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
        private List<GuiLabel> labelList;
        private int currentTop;
        private int displayableItemsCount;
        private int itemHeight;
        private int itemWidth;
        private int maxWidth;

        public static GuiList createNewList(Point position, int displayableItems, List<GuiLabel> labels, int maxWidth = -1)
        {
            // Create a container for the width of the display
            int width = maxWidth;

            // If the max width is bogus, calculate the width
            if(width < 16)
            {
                width = 16;

                // Get the biggest width of labels
                foreach (GuiLabel label in labels)
                {
                    width = Math.Max(label.Bounds.Width + 16, width);
                }
            }

            // Get the height of the list.
            int height = 0;
            try
            {
                height = labels[0].Bounds.Height * displayableItems;
            }
            catch (ArgumentOutOfRangeException)
            {
                height = 32;
            }
            // Return a new GuiList based on position, width and height. 
            GuiList retVal = new GuiList(new Rectangle(position, new Point(width, height)), labels);
            retVal.displayableItemsCount = displayableItems;
            retVal.itemHeight = height / displayableItems;
            retVal.itemWidth = width - 16;
            retVal.calculateLabelPositions();
            return retVal;
        }

        private GuiList(Rectangle bounds, List<GuiLabel> labels) : base(bounds)
        {
            // Create the up-button
            up = new GuiButton(new Rectangle(new Point(0, 0), new Point(16)), "testure");
            up.ClickHandler = () => scroll(true);
            up.loadContent(AssetManager.Instance);

            // Create the down-button
            down = new GuiButton(new Rectangle(new Point(0, 0), new Point(16)), "testure");
            down.ClickHandler = () => scroll(false);
            down.loadContent(AssetManager.Instance);

            // Create the list-related variables
            labelList = labels;
            currentTop = 0;

            // Make sure all labels know this is their parent
            foreach (GuiLabel label in labelList)
            {
                label.Parent = this;
            }

            // Make sure the buttons know this is their parent
            up.Parent = this;
            down.Parent = this;

            maxWidth = bounds.Width;
        }

        private bool recalculateBounds()
        {
            if (labelList.Count <= 0)
            {
                Visible = false;
                return false;
            }

            // Buttons should be visible only when we have to scroll...
            up.Visible = down.Visible = labelList.Count > displayableItemsCount;

            itemHeight = labelList[0].Bounds.Height;
            int height = itemHeight * Math.Min(labelList.Count, displayableItemsCount);
            int width = maxWidth;

            Bounds = new Rectangle(Bounds.Location, new Point(width, height));

            // We're visible, this should be fine
            return true;
        }

        public void calculateLabelPositions()
        {
            // If we're not visible, just don't do anything at all.
            if (!recalculateBounds())
            {
                return;
            }

            // Assume we're visible
            bool visible = true;

            // Set every label's position
            for (int i = 0; i < labelList.Count; ++i)
            {
                // Set the location of the label to its position in the list
                labelList[i].move(new Point(Bounds.Location.X, Bounds.Location.Y - (currentTop - i) * itemHeight));
                if (!labelList[i].Visible)
                    visible = false;
            }
            Visible = visible;

            up.move(new Point(Bounds.Right - 16, Bounds.Top));
            down.move(new Point(Bounds.Right - 16, Bounds.Bottom - 16));
        }

        public void clear()
        {
            this.labelList.Clear();
            calculateLabelPositions();
        }

        private void scroll(bool up)
        {
            int move = up ? -1 : 1;

            if (currentTop + move < 0 || currentTop + move + displayableItemsCount > labelList.Count)
            {
                return;
            }

            currentTop += move;
            calculateLabelPositions();
        }

        public override void drawElement(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            for (int i = currentTop; i < currentTop + displayableItemsCount && i < labelList.Count; ++i)
            {
                labelList[i].drawElement(spriteBatch);
            }
            up.drawElement(spriteBatch);
            down.drawElement(spriteBatch);
        }

        public override void loadContent(AssetManager content)
        {
            up.loadContent(content);
            down.loadContent(content);
            foreach (GuiLabel label in labelList)
                label.loadContent(content);
        }

        public override void onClick(ClickEvent e)
        {
            if (up.Bounds.Contains(e.Position))
                up.onClick(e);

            if (down.Bounds.Contains(e.Position))
                down.onClick(e);
        }

        public void addLabel(GuiLabel label, int index = -1)
        {
            // Add the label, possibly at a specified index
            if (index < 0 || index >= labelList.Count)
                labelList.Add(label);
            else
                labelList.Insert(index, label);

            // Make sure all labels are in the correct positions
            calculateLabelPositions();
        }

        public void removeLabel(int index)
        {
            // Only remove the label if it's in a valid position
            if (index > 0 && index < labelList.Count)
            {
                labelList.RemoveAt(index);
            }
        }

        // TODO: see if this works as intended
        public void removeLabel(GuiLabel label)
        {
            int index = labelList.FindIndex(gl => label.Equals(gl));
            removeLabel(index);
        }

        public List<GuiLabel> AllLabels => labelList;
    }
}
