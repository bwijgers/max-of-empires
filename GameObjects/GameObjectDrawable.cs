using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui;

namespace MaxOfEmpires.GameObjects
{
    class GameObjectDrawable : GameObject
    {
        private Color drawColor;
        private Vector2 drawPosition;
        private Vector2 size;
        private Texture2D tex;

        public GameObjectDrawable()
        {
            tex = DrawingHelper.PixelTexture;
            drawColor = Color.White;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            s.Draw(tex, DrawPosition, drawColor);
        }

        public Color DrawColor
        {
            get
            {
                return drawColor;
            }
            set
            {
                drawColor = value;
            }
        }

        public Texture2D DrawingTexture
        {
            get
            {
                return tex;
            }
            set
            {
                tex = value;
            }
        }

        public virtual Vector2 DrawPosition
        {
            get
            {
                return drawPosition;
            }
            set
            {
                drawPosition = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                return tex.Bounds.Size.ToVector2();
            }
        }
    }
}
