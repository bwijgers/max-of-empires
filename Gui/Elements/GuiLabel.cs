using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MaxOfEmpires;

namespace Ebilkill.Gui.Elements
{
    /// <summary>
    /// Objects of this class are the way for a GuiScreen to draw text on the screen. 
    /// </summary>
    class GuiLabel : GuiElement
    {
        private string fontName;
        private string labelText;
        private SpriteFont font;
        private Color labelColor;

        private static Vector2 getLabelSize(GuiLabel label)
        {
            // Make sure the font exists; no font means just padding
            if (label.font == null)
                return PaddingSize;

            // Get the size of the string and add the padding
            Vector2 stringSize = label.font.MeasureString(label.labelText);
            return PaddingSize + stringSize;
        }

        /// <summary>
        /// Creates a new black <code>GuiLabel</code>, based on the position, the text and the font supplied.
        /// </summary>
        /// <param name="position">The screen coords to place this <code>GuiLabel</code>.</param>
        /// <param name="labelText">The text this <code>GuiLabel</code> should display.</param>
        /// <param name="fontName">The name of the <code>SpriteFont</code> this <code>GuiLabel</code> should use.</param>
        /// <returns>The <code>GuiLabel</code> to create.</returns>
        public static GuiLabel createNewLabel(Vector2 position, string labelText, string fontName)
        {
            return createNewLabel(position, labelText, fontName, Color.Black);
        }

        /// <summary>
        /// Creates a new black <code>GuiLabel</code>, based on the position, the text and the font supplied.
        /// </summary>
        /// <param name="position">The screen coords to place this <code>GuiLabel</code>.</param>
        /// <param name="labelText">The text this <code>GuiLabel</code> should display.</param>
        /// <param name="fontName">The name of the <code>SpriteFont</code> this <code>GuiLabel</code> should use.</param>
        /// <param name="labelColor">The <code>Color</code> this <code>GuiLabel</code> should be.</param>
        /// <returns>The <code>GuiLabel</code> to create.</returns>
        public static GuiLabel createNewLabel(Vector2 position, string labelText, string fontName, Color labelColor)
        {
            GuiLabel label = new GuiLabel(new Rectangle(), labelText, fontName, labelColor);
            label.loadContent(AssetManager.Instance);
            label.calculateBounds(position);
            return label;
        }

        /// <summary>
        /// Private constructor so that the static <code>createNewLabel</code> method must be used.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="labelText"></param>
        /// <param name="fontName"></param>
        /// <param name="labelColor"></param>
        private GuiLabel(Rectangle bounds, string labelText, string fontName, Color labelColor) : base(bounds)
        {
            this.labelText = labelText;
            this.fontName = fontName;
            this.labelColor = labelColor;
        }

        public override void drawElement(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;
            spriteBatch.DrawString(font, labelText, Bounds.Location.ToVector2() + PaddingSize / 2, labelColor);
        }

        public override void loadContent(AssetManager content)
        {
            this.font = content.getAsset<SpriteFont>(this.fontName);
        }

        private void calculateBounds(Vector2 position)
        {
            Vector2 size = getLabelSize(this);
            this.Bounds = new Rectangle(position.ToPoint(), size.ToPoint());
        }

        public void setLabelText(string text)
        {
            this.labelText = text;
            this.calculateBounds(Bounds.Location.ToVector2());
        }

        public override void onVisibilityChange(bool newVisibility)
        {
            base.onVisibilityChange(newVisibility);
            if (Parent is GuiList)
                (Parent as GuiList).calculateLabelPositions();
        }

        public override bool Visible
        {
            get
            {
                return base.Visible && font != null;
            }

            set
            {
                base.Visible = value;
            }
        }

        public SpriteFont Font
        {
            get
            {
                return font;
            }
            set
            {
                if (value != null)
                    font = value;
            }
        }

        private static Vector2 PaddingSize => new Vector2(8);
    }
}
