using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameObjects
{
    class GameObjectAnimated : GameObjectDrawable
    {
        private bool advanceRows;
        private Animation animation;
        protected bool shouldAnimate;
        private double animationTime;

        public GameObjectAnimated(bool advanceRows, double animationTime)
        {
            this.advanceRows = advanceRows;
            animation = new Animation(DrawingTexture, advanceRows, animationTime);
            this.animationTime = animationTime;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            base.Draw(time, s);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            // Only animate if we should animate 
            if (!shouldAnimate)
                return;

            animation.Update(time);
        }

        public void ResetAnimation()
        {
            animation.Spritesheet.SelectedSprite = new Point(0, 0);
        }

        public override Spritesheet DrawingTexture
        {
            get
            {
                if (animation != null)
                    return animation.Spritesheet;

                return base.DrawingTexture;
            }

            set
            {
                animation = new Animation(value, advanceRows, animationTime);
            }
        }

        public bool ShouldAnimate
        {
            get
            {
                return shouldAnimate;
            }
            set
            {
                if (!value)
                    ResetAnimation();
                shouldAnimate = value;
            }
        }
    }
}