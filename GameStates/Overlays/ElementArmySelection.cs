using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using Ebilkill.Gui.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameStates.Overlays
{
    class ElementArmySelection : GuiElement
    {
        /// <summary>
        /// Creates a new element with a label and a button, allowing a player to build a building using a builder.
        /// </summary>
        /// <param name="startPos">The starting position of this element on the screen.</param>
        /// <param name="unitNameWithCount">The string in the label.</param>
        /// <param name="addSelectionHandler">The click handler for the button.</param>
        /// <returns>The element with the label and the button.</returns>
        public static ElementArmySelection CreateBuildButton(Point startPos, string unitNameWithCount, GuiButton.OnClickHandler addSelectionHandler, GuiButton.OnClickHandler removeSelectionHandler)
        {
            // Create the new element using the starting position, and add the click handler
            Rectangle rect = new Rectangle(startPos, new Point(0));
            ElementArmySelection retVal = new ElementArmySelection(startPos, unitNameWithCount, " - ", " + ");
            retVal.buttonRemoveSelection.ClickHandler = removeSelectionHandler;
            retVal.buttonAddSelection.ClickHandler = addSelectionHandler;

            // Calculate the location and size of this object
            Point location = new Point(retVal.labelUnitSelection.Bounds.Left, retVal.buttonRemoveSelection.Bounds.Top);
            Point size = new Point(retVal.buttonRemoveSelection.Bounds.Right - location.X, retVal.buttonRemoveSelection.Bounds.Bottom - location.Y + 5);
            retVal.Bounds = new Rectangle(location, size);

            // Return the newly generated object
            return retVal;
        }

        // The two GuiElements this element is made up of
        private GuiButton buttonAddSelection;
        private GuiButton buttonRemoveSelection;
        private GuiLabel labelUnitSelection;

        /// <summary>
        /// Private constructor so the static creation method must be used.
        /// </summary>
        /// <param name="startPos">The starting position of this element on the screen.</param>
        /// <param name="buildingString">The string in the label.</param>
        private ElementArmySelection(Point startPos, string buildingString, string removeText, string addText) : base(new Rectangle(startPos, Point.Zero))
        {
            labelUnitSelection = GuiLabel.createNewLabel(startPos.ToVector2(), buildingString, "font");
            buttonAddSelection = GuiButton.createButtonWithLabel(new Point(labelUnitSelection.Bounds.Right + 15, labelUnitSelection.Bounds.Top), addText, null, "font");
            buttonRemoveSelection = GuiButton.createButtonWithLabel(new Point(buttonAddSelection.Bounds.Right + 15, labelUnitSelection.Bounds.Top), removeText, null, "font");
        }

        public override void drawElement(SpriteBatch s)
        {
            // Just draw both elements
            labelUnitSelection.drawElement(s);
            buttonRemoveSelection.drawElement(s);
            buttonAddSelection.drawElement(s);
        }

        public override void loadContent(AssetManager content)
        {
            // Just load the content for both elements
            labelUnitSelection.loadContent(content);
            buttonRemoveSelection.loadContent(content);
            buttonAddSelection.loadContent(content);
        }

        public override void move(Point pos)
        {
            // Move this element
            base.move(pos);

            // Find the distance between both elements
            int distanceToRemove = buttonRemoveSelection.Bounds.Left - labelUnitSelection.Bounds.Left;
            int distanceToAdd = buttonAddSelection.Bounds.Left - labelUnitSelection.Bounds.Left;

            // Move the child elements
            labelUnitSelection.move(new Point(Bounds.Location.X, Bounds.Location.Y + 2)); // Move the label down a little; it looks better
            buttonRemoveSelection.move(new Point(Bounds.Left + distanceToRemove, Bounds.Top));
            buttonAddSelection.move(new Point(Bounds.Left + distanceToAdd, Bounds.Top));
        }

        public override void onClick(ClickEvent e)
        {
            // Pass on the click event to the button child
            if (buttonRemoveSelection.Bounds.Contains(e.Position))
                buttonRemoveSelection.onClick(e);

            if (buttonAddSelection.Bounds.Contains(e.Position))
                buttonAddSelection.onClick(e);
        }
    }
}
