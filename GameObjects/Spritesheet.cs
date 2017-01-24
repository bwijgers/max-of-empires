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
        private bool padded;

        public Spritesheet(Texture2D texture, int width, int height, bool padded = true)
        {
            fullTexture = texture;
            columns = width;
            rows = height;
            elementSize = new Point(texture.Width / columns, texture.Height / rows);
            SelectedSprite = new Point(0, 0);
            this.padded = padded;
        }

        public void Draw(GameTime time, SpriteBatch s, Vector2 destination, Color color)
        {
            Point spriteSize = Size.ToPoint();
            Point spriteStart = new Point(spriteSize.X * currentColumn, spriteSize.Y * currentRow);
            Rectangle spritePart = new Rectangle(spriteStart, spriteSize);

            if (padded)
                destination -= spriteSize.ToVector2() / 3;

            s.Draw(fullTexture, destination, spritePart, color);
        }

        public void SelectNextSprite(bool advanceRows, bool repeating)
        {
            ++currentColumn;
            if (currentColumn == columns)
            {
                if (repeating)
                {
                    currentColumn = 0;
                    if (advanceRows)
                    {
                        ++currentRow;
                        if (currentRow == rows)
                        {
                            currentRow = 0;
                        }
                    }
                }
            }
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
