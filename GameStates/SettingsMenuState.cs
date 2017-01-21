using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MaxOfEmpires.GameStates.Overlays;

namespace MaxOfEmpires.GameStates
{
    class SettingsMenuState : GameState
    {
        private OverlaySettingsMenu overlay;

        public SettingsMenuState()
        {
            ResetOverlay();
        }

        public override void Draw(GameTime time, SpriteBatch gameObjectS, SpriteBatch overlayS)
        {
            overlay.draw(overlayS);
        }

        public override void HandleInput(InputHelper helper, KeyManager manager)
        {
            overlay.update(helper);
        }

        public override void ResetOverlay()
        {
            overlay = new OverlaySettingsMenu();
        }

        public override void Update(GameTime time)
        {
        }
    }
}