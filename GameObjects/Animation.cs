using Microsoft.Xna.Framework;

namespace MaxOfEmpires.GameObjects
{
    class Animation
    {
        private bool advanceRows;
        private Spritesheet sheet;
        private readonly double TIME_BETWEEN_FRAMES;
        private double timeThisFrame;

        public Animation(Spritesheet sheet, bool advanceRows, double timeBetweenFrames )
        {
            this.sheet = sheet;
            this.advanceRows = advanceRows;
            TIME_BETWEEN_FRAMES = timeBetweenFrames;
            timeThisFrame = 0;
        }

        public void Update(GameTime time)
        {
            timeThisFrame += time.ElapsedGameTime.TotalSeconds;
            if (timeThisFrame > TIME_BETWEEN_FRAMES)
            {
                timeThisFrame -= TIME_BETWEEN_FRAMES;
                sheet.SelectNextSprite(advanceRows);
            }
        }

        public Spritesheet Spritesheet => sheet;
    }
}
