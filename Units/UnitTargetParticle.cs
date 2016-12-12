using System;
using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.Units
{
    class UnitTargetParticle : GameObject
    {
        private const double TIME_TO_LIVE = 0.5D; // seconds
        private Vector2 drawPos;
        private Texture2D tex;
        private double timeExisted;

        public UnitTargetParticle(Vector2 drawPos)
        {
            this.drawPos = drawPos;
            tex = AssetManager.Instance.getAsset<Texture2D>("TargetParticle");
            timeExisted = 0;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            Color drawColor = new Color(0xFF, 0x00, 0xFF, (float) Math.Sin(timeExisted / TIME_TO_LIVE * Math.PI));
            s.Draw(tex, drawPos, drawColor);
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