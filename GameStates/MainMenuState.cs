using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MaxOfEmpires.GameStates.Overlays;

namespace MaxOfEmpires.GameStates
{
    class MainMenuState : GameState
    {
        private OverlayMainMenu overlay;

        public MainMenuState()
        {
            ResetOverlay();
        }

        public override void Draw(GameTime time, SpriteBatch gameObjectS, SpriteBatch overlayS)
        {
            overlay.draw(overlayS);
            base.Draw(time, gameObjectS, overlayS);
        }

        public override void HandleInput(InputHelper helper, KeyManager manager)
        {
            overlay.update(helper);
        }

        public override void ResetOverlay()
        {
            overlay = new OverlayMainMenu();
        }
    }
}
