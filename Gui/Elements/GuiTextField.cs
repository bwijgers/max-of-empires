using MaxOfEmpires;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace Ebilkill.Gui.Elements
{
    public class GuiTextField : GuiElement
    {
        private string fontName;
        private SpriteFont font;
        private string text;
        private int maxChars;

        public GuiTextField(Rectangle bounds) : base(bounds)
        {
            fontName = "font";
            text = "";
        }

        public override void drawElement(SpriteBatch spriteBatch)
        {
            DrawingHelper.Instance.DrawRectangle(spriteBatch, Bounds, Color.Black);
            DrawingHelper.Instance.DrawRectangle(spriteBatch, new Rectangle(Bounds.Location + new Point(1, 1), Bounds.Size - new Point(2, 2)), Color.White);
            spriteBatch.DrawString(font, text, Bounds.Location.ToVector2(), Color.Black);
        }

        public override void handleKeyboardInput(InputHelper helper)
        {
            bool changed = false;
            foreach (Keys k in helper.TextKeys.Keys)
            {
                if (helper.KeyPressed(k, false))
                {
                    char toAdd = helper.TextKeys[k];

                    if (helper.IsKeyDown(Keys.LeftShift) || helper.IsKeyDown(Keys.RightShift))
                    {
                        toAdd = new StringBuilder().Append(toAdd).ToString().ToUpper()[0];
                    }

                    text += toAdd;
                    changed = true;
                }
            }

            if (changed)
                calculateBounds();
        }

        private void calculateBounds()
        {
            Vector2 size = font.MeasureString(text);
            this.Bounds = new Rectangle(Bounds.Location, size.ToPoint());
        }

        public override void loadContent(AssetManager content)
        {
            this.font = content.getAsset<SpriteFont>(fontName);
        }
    }
}
