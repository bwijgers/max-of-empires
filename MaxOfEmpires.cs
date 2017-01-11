using Ebilkill.Gui;
using MaxOfEmpires.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MaxOfEmpires
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MaxOfEmpires : Game
    {
        private static GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputHelper inputHelper;
        private static Random random = new Random((int) DateTime.Now.Ticks);
        private static bool running = true;
        public static Camera camera = new Camera();

        public MaxOfEmpires()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            inputHelper = new InputHelper();

            GraphicsDevice.Viewport = new Viewport(0, 0, 1280, 768);
            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Init AssetManager and DrawingHelper
            AssetManager.Init(Content);
            DrawingHelper.Init(GraphicsDevice);

            // Adds battleState to the GamestateManager
            GameStateManager.AddState("battle", new BattleState());
            GameStateManager.AddState("mainMenu", new MainMenuState());
            GameStateManager.SwitchState("mainMenu");

            // Initialize the key inputs
            InitializeKeys();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!running)
            {
                Exit();
            }

            inputHelper.Update(gameTime);
            camera.Update(gameTime, inputHelper, KeyManager.Instance);

            // Update current gamestate
            GameStateManager.Update(gameTime);
            GameStateManager.HandleInput(inputHelper, KeyManager.Instance);

            //TIJDELIJKE CODE TOT WE SETTINGS HEBBEN OM TE SWITCHEN TUSSEN MOUSE EN KEYBOARD CONTROLL VAN DE CAMERA
            if (inputHelper.KeyPressed(Keys.K))
            {
                camera.useMouse = !camera.useMouse;
            }
            //EINDE TIJDELIJKE CODE

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            GameStateManager.Draw(gameTime, spriteBatch);// Draw the current game state
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void InitializeKeys()
        {
            KeyManager.Instance.RegisterKey("unitTargetOverlay", Keys.T);
            KeyManager.Instance.RegisterKey("moveCameraUp", Keys.Up);
            KeyManager.Instance.RegisterKey("moveCameraDown", Keys.Down);
            KeyManager.Instance.RegisterKey("moveCameraLeft", Keys.Left);
            KeyManager.Instance.RegisterKey("moveCameraRight", Keys.Right);
        }

        public static void Quit()
        {
            running = false;
        }

        public static Random Random => random;

        public static Point ScreenSize => new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
    }
}
