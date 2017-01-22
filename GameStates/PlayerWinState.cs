using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameStates
{
    class PlayerWinState : GameState
    {
        private Player winningPlayer;

        public override Vector2 GetCurrentGridSize()
        {
            return Vector2.Zero;
        }

        public override void Draw(GameTime time, SpriteBatch gameObjectS, SpriteBatch overlayS)
        {
            //overlayS.Draw(AssetManager.Instance.getAsset<Texture2D>("Victoryscreen"), new Rectangle(Point.Zero, MaxOfEmpires.ScreenSize), winningPlayer.);
        }

        public void OnPlayerWinGame(Player winningPlayer)
        {
            this.winningPlayer = winningPlayer;
        }
    }
}
