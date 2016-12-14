using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebilkill.Gui
{
    public class DrawingHelper
    {
        private static DrawingHelper instance;
        private static Texture2D whiteTex;

        public static void Init(GraphicsDevice gd)
        {
            if (instance != null)
                return;

            instance = new DrawingHelper();
            whiteTex = new Texture2D(gd, 1, 1);
            whiteTex.SetData(new Color[] { new Color(255, 255, 255, 255) });
        }

        private DrawingHelper()
        {
        }

        public void DrawRectangle(SpriteBatch s, Rectangle rect, Color? c)
        {
            if (c == null || !c.HasValue)
                c = Color.White;

            s.Draw(whiteTex, rect, c.Value);
        }

        public static DrawingHelper Instance => instance;

        public static Texture2D PixelTexture => whiteTex;
    }
}
