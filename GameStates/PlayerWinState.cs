using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameStates
{
    class PlayerWinState : GameState
    {
        private double displayTime;
        private bool switchingState;
        private const double TOTAL_DISPLAY_TIME = 10.0D;
        private Player winningPlayer;
        private Texture2D winningScreen;

        public PlayerWinState()
        {
            winningScreen = AssetManager.Instance.getAsset<Texture2D>("Victoryscreen");
        }

        public override void Draw(GameTime time, SpriteBatch gameObjectS, SpriteBatch overlayS)
        {
            overlayS.Draw(winningScreen, new Rectangle(Point.Zero, MaxOfEmpires.ScreenSize), winningPlayer.Color);
            base.Draw(time, gameObjectS, overlayS);
        }

        public void OnPlayerWinGame(Player winningPlayer)
        {
            this.winningPlayer = winningPlayer;
            displayTime = 0;
            switchingState = false;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            displayTime += time.ElapsedGameTime.TotalSeconds;
            if (displayTime >= TOTAL_DISPLAY_TIME && !switchingState)
            {
                //GameStateManager.OnRequestStatistics();
                switchingState = true;
            }
        }
    }
}
