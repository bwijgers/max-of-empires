using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MaxOfEmpires.GameObjects
{
    class Spritesheet
    {
        private Point elementSize;
        private Texture2D fullTexture;
        private int columns;
        private int rows;
        private int currentColumn, currentRow;

        public Spritesheet(Texture2D texture, int width, int height)
        {
            fullTexture = texture;
            columns = width;
            rows = height;
            elementSize = new Point(texture.Width / columns, texture.Height / rows);
            SelectedSprite = new Point(0, 0);
        }

        public void Draw(GameTime time, SpriteBatch s, Vector2 destination, Color color)
        {
            Point spriteSize = Size.ToPoint();
            Point spriteStart = new Point(spriteSize.X * currentColumn, spriteSize.Y * currentRow);
            Rectangle spritePart = new Rectangle(spriteStart, spriteSize);

            s.Draw(fullTexture, destination, spritePart, color);
        }

        public Point SelectedSprite
        {
            get
            {
                return new Point(currentColumn, currentRow);
            }
            set
            {
                currentColumn = MathHelper.Clamp(value.X, 0, columns - 1);
                currentRow = MathHelper.Clamp(value.Y, 0, rows - 1);
            }
        }
        public Vector2 Size => new Vector2(fullTexture.Width / columns, fullTexture.Height / rows);
    }
}
