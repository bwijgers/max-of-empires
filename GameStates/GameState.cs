using Ebilkill.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.GameStates
{
    abstract class GameState
    {
        public delegate void FadeoutEndCallback();

        /// <summary>
        /// The percentage of fade in / fade out. A value between 0 and 1 defining how faded the screen should be.
        /// </summary>
        protected double fadeAmount = 0;

        /// <summary>
        /// An int defining whether we should be fading in or out.
        /// </summary>
        private int fading = 0;

        /// <summary>
        /// The functions that are executed when fadeout ended. Set to null after the fadeout ends.
        /// </summary>
        public FadeoutEndCallback fadeoutEndCallback;

        public virtual void Draw(GameTime time, SpriteBatch gameObjectS, SpriteBatch overlayS)
        {
            if (fadeAmount != 0)
                DrawingHelper.Instance.DrawRectangle(overlayS, new Rectangle(Point.Zero, MaxOfEmpires.ScreenSize), new Color(0, 0, 0, (float)fadeAmount));
        }

        public virtual void HandleInput(InputHelper helper, KeyManager manager)
        {
        }

        /// <summary>
        /// Calles the fadeoutEndCallback when the fadeout actually ended.
        /// </summary>
        private void OnFadeoutEnd()
        {
            if (fadeoutEndCallback == null)
                return;

            fadeoutEndCallback();
            fadeoutEndCallback = null;
        }

        public virtual void Reset()
        {
            ResetOverlay();
        }

        public virtual void ResetOverlay()
        {
        }

        public virtual void Update(GameTime time)
        {
            // If we're fading in... 
            if (FadeIn)
            {
                // Check if the fade in has ended
                if (fadeAmount <= 0)
                {
                    // if it has, set it to false and stop executing
                    FadeIn = false;
                    fadeAmount = 0;
                    return;
                }

                // if it's not done, continue fading in.
                fadeAmount -= time.ElapsedGameTime.Milliseconds / 2000.0D;
            }

            // If we're not fading in, do the same thing (essentially) for fading out.
            else if (FadeOut)
            {
                if (fadeAmount >= 1)
                {
                    FadeOut = false;
                    fadeAmount = 1;
                    return;
                }
                fadeAmount += time.ElapsedGameTime.Milliseconds / 2000.0D;
            }
        }

        /// <summary>
        /// True when fading in, false when not fading in.
        /// </summary>
        public bool FadeIn
        {
            get
            {
                return fading < 0;
            }
            set
            {
                if (value)
                {
                    fading = -1;
                    fadeAmount = 1;
                    return;
                }
                fading = 0;
            }
        }

        /// <summary>
        /// True when fading out, false when not fading out.
        /// </summary>
        public bool FadeOut
        {
            get
            {
                return fading > 0;
            }
            set
            {
                if (value)
                {
                    fading = 1;
                    fadeAmount = 0;
                    return;
                }

                OnFadeoutEnd();
                fading = 0;
            }
        }
    }
}
