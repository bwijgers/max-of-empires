using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MaxOfEmpires.GameStates.Overlays;
using System;

namespace MaxOfEmpires.GameStates
{
    class MainMenuState : GameState
    {
        private OverlayMainMenu overlay;
        private static Random r = new Random();
        private Texture2D mainMenuTex = AssetManager.Instance.getAsset<Texture2D>("TitleScreen/titlescreen" + (r.Next(0, 3) +1 ).ToString());
        private Texture2D settingsButton = AssetManager.Instance.getAsset<Texture2D>("TitleScreen/SettingsButton");
        private Player Blue;
        private Player Red;

        public MainMenuState(Player blue, Player red)
        {
            ResetOverlay();
            Blue = blue;
            Red = red;

        }

        public override void Draw(GameTime time, SpriteBatch gameObjectS, SpriteBatch overlayS)
        {
            gameObjectS.Draw(mainMenuTex, new Rectangle(new Point(0,0), MaxOfEmpires.ScreenSize), Color.White);
            overlay.draw(overlayS);
            base.Draw(time, gameObjectS, overlayS);
        }

        public override void HandleInput(InputHelper helper, KeyManager manager)
        {
            overlay.update(helper);
        }

        public override void ResetOverlay()
        {
            overlay = new OverlayMainMenu(Blue, Red);
            overlay.loadContent(AssetManager.Instance);
        }
    }
}
