using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui;

namespace MaxOfEmpires.GameObjects
{
    class GameObjectDrawable : GameObject
    {
        private Color drawColor;
        private Vector2 size;
        private Spritesheet tex;

        public GameObjectDrawable()
        {
            tex = new Spritesheet(DrawingHelper.PixelTexture, 1, 1);
            drawColor = Color.White;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            DrawingTexture.Draw(time, s, DrawPosition, drawColor);
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(DrawPosition.ToPoint(), Size.ToPoint());
            }
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

        public virtual Spritesheet DrawingTexture
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

        public virtual Vector2 Size
        {
            get
            {
                return tex.Size;
            }
        }
    }
}
