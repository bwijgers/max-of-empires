using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui.Events;
using MaxOfEmpires;

/// <summary>
/// -----------------------------------------------
/// -------------------Button.cs-------------------
/// ---------------Made by: CallitMez--------------
/// ----------------Made for RPGGame---------------
/// -------------------12-10-2016------------------
/// ------------github.com/CallitMez/RPG-----------
/// -----------------------------------------------
/// </summary>

namespace Ebilkill.Gui.Elements
{
    class GuiButton : GuiElement
    {
        public delegate void OnClickHandler();

        private Texture2D sprite;
        private GuiLabel label;
        private OnClickHandler clickHandler;
        private string spriteName;

        public static GuiButton createButtonWithLabel(GuiLabel label, string spriteName)
        {
            Rectangle bounds = new Rectangle(label.Bounds.Location - new Point(2, 2), label.Bounds.Size + new Point(4, 4));
            GuiButton retVal = new GuiButton(bounds, spriteName);
            retVal.label = label;
            return retVal;
        }

        public static GuiButton createButtonWithLabel(Point topLeft, string text, string spriteName, string fontName)
        {
            GuiLabel label = GuiLabel.createNewLabel(topLeft.ToVector2() + new Vector2(2, 2), text, fontName);
            return createButtonWithLabel(label, spriteName);
        }

        public GuiButton(Rectangle bounds, string spriteName) : base(bounds)
        {
            this.spriteName = spriteName;
        }

        public override void loadContent(AssetManager content)
        {
            sprite = content.getAsset<Texture2D>(spriteName);
        }

        public override void drawElement(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (!Visible)
                return;
            spriteBatch.Draw(sprite, Bounds, Color.White);
            if (label != null)
                label.drawElement(spriteBatch, graphics);
        }

        public override void onClick(ClickEvent e)
        {
            clickHandler();
        }

        public OnClickHandler ClickHandler
        {
            set
            {
                this.clickHandler = value;
            }
        }
    }
}
