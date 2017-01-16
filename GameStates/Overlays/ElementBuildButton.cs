using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using Ebilkill.Gui.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameStates.Overlays
{
    class ElementBuildButton : GuiElement
    {
        /// <summary>
        /// Creates a new element with a label and a button, allowing a player to build a building using a builder.
        /// </summary>
        /// <param name="startPos">The starting position of this element on the screen.</param>
        /// <param name="buildingString">The string in the label.</param>
        /// <param name="clickHandler">The click handler for the button.</param>
        /// <returns>The element with the label and the button.</returns>
        public static ElementBuildButton CreateBuildButton(Point startPos, string buildingString, GuiButton.OnClickHandler clickHandler)
        {
            Rectangle rect = new Rectangle(startPos, new Point(0));
            ElementBuildButton retVal = new ElementBuildButton(startPos, buildingString);
            retVal.buttonBuildBuilding.ClickHandler = clickHandler;
            retVal.Bounds = new Rectangle(retVal.labelBuildingNameAndCost.Bounds.Left, retVal.labelBuildingNameAndCost.Bounds.Top, retVal.buttonBuildBuilding.Bounds.Right, retVal.buttonBuildBuilding.Bounds.Bottom);
            return retVal;
        }

        // The two GuiElements this element is made up of
        private GuiButton buttonBuildBuilding;
        private GuiLabel labelBuildingNameAndCost;

        /// <summary>
        /// Private constructor so the static creation method must be used.
        /// </summary>
        /// <param name="startPos">The starting position of this element on the screen.</param>
        /// <param name="buildingString">The string in the label.</param>
        private ElementBuildButton(Point startPos, string buildingString) : base(new Rectangle(startPos, Point.Zero))
        {
            labelBuildingNameAndCost = GuiLabel.createNewLabel(startPos.ToVector2(), buildingString, "font");
            buttonBuildBuilding = GuiButton.createButtonWithLabel(new Point(labelBuildingNameAndCost.Bounds.Right + 5, labelBuildingNameAndCost.Bounds.Top), "Build", null, "font");
        }

        public override void drawElement(SpriteBatch s)
        {
            // Just draw both elements
            labelBuildingNameAndCost.drawElement(s);
            buttonBuildBuilding.drawElement(s);
        }

        public override void loadContent(AssetManager content)
        {
            // Just load the content for both elements
            labelBuildingNameAndCost.loadContent(content);
            buttonBuildBuilding.loadContent(content);
        }

        public override void onClick(ClickEvent e)
        {
            // Pass on the click event to the button child
            if (buttonBuildBuilding.Bounds.Contains(e.Position))
                buttonBuildBuilding.onClick(e);
        }
    }
}
