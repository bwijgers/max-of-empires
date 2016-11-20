using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MaxOfEmpires;

namespace Ebilkill.Gui
{
    class ScreenManager
    {
        private static ScreenManager instance = new ScreenManager();
        public Dictionary<string, GuiScreen> screenDict = new Dictionary<string, GuiScreen>();
        private string currentScreen;

        private ScreenManager()
        {
            currentScreen = "";
        }

        public void loadContent(AssetManager assetManager)
        {
            foreach(GuiScreen s in screenDict.Values)
            {
                s.loadContent(assetManager);
            }
        }

        public void draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            CurrentScreen?.draw(spriteBatch, graphicsDevice);
        }

        public void update(GameTime gameTime, InputHelper inputHelper)
        {
            foreach (GuiScreen s in screenDict.Values)
            {
                s.update(gameTime, inputHelper);
            }
        }

        public void selectScreen(string screen)
        {
            if (screenDict.ContainsKey(screen))
            {
                currentScreen = screen;
                return;
            }
            throw new KeyNotFoundException("Screen '" + screen + "' is unknown.");
        }

        public GuiScreen CurrentScreen
        {
            get
            {
                return currentScreen == "" ? null : screenDict[currentScreen];
            }
        }

        public static ScreenManager Instance => instance;
    }
}
