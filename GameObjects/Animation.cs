using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameObjects
{
    class Animation
    {
        private Spritesheet sheet;
        private readonly double TIME_BETWEEN_FRAMES;
        private double timeThisFrame;

        public Animation(Spritesheet sheet, double timeBetweenFrames = 0.30D)
        {
            this.sheet = sheet;
            TIME_BETWEEN_FRAMES = timeBetweenFrames;
            timeThisFrame = 0;
        }

        public void Draw(GameTime time, SpriteBatch s, Vector2 destination, Color color)
        {
            sheet.Draw(time, s, destination, color);
        }

        public void Update(GameTime time)
        {
            timeThisFrame += time.ElapsedGameTime.TotalSeconds;
            while (timeThisFrame > TIME_BETWEEN_FRAMES)
            {
                timeThisFrame -= TIME_BETWEEN_FRAMES;
                sheet.SelectNextSprite();
            }
        }
    }
}
