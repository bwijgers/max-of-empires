using System;
using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.Units
{
    class TargetPositionParticle : GameObjectDrawable
    {
        private const double TIME_TO_LIVE = 0.5D; // seconds
        private double timeExisted;

        public TargetPositionParticle(Vector2 drawPos)
        {
            DrawPosition = drawPos;
            DrawingTexture = AssetManager.Instance.getAsset<Spritesheet>("TargetParticle");
            timeExisted = 0;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            // Change alpha based on how long we still exist
            DrawColor = new Color(0xFF, 0x00, 0xFF, (float) Math.Sin(timeExisted / TIME_TO_LIVE * Math.PI));
            base.Draw(time, s);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            timeExisted += time.ElapsedGameTime.TotalSeconds;
            if (timeExisted >= TIME_TO_LIVE)
            {
                (Parent as GameObjectList).RemoveChild(this);
            }
        }
    }
}