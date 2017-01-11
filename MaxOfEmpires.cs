using Ebilkill.Gui;
using MaxOfEmpires.Files;
using MaxOfEmpires.GameStates;
using MaxOfEmpires.Units;
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
        private Configuration mainConfiguration;
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

        protected void LoadConfiguration()
        {
            // Initialize main configuration file
            mainConfiguration = FileManager.LoadConfig("Main");

            // Initialize units
            Configuration unitConfiguration = mainConfiguration.GetPropertySection("unit");
            UnitRegistry.Init(unitConfiguration);

            // Initialize keys
            InitializeKeys(FileManager.LoadConfig("Keys").GetPropertySection("key"));
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

            // Load the main configuration file, load all subconfigs
            LoadConfiguration();

            // Adds battleState to the GamestateManager
            GameStateManager.AddState("battle", new BattleState());
            GameStateManager.AddState("mainMenu", new MainMenuState());
            GameStateManager.SwitchState("mainMenu");
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
            GameStateManager.Draw(gameTime, spriteBatch); // Draw the current game state
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Initializes all keybinds, loaded from Keys.cfg
        /// </summary>
        /// <param name="config">The configuration file and section to use for loading.</param>
        private void InitializeKeys(Configuration config)
        {
            KeyManager.Instance.RegisterKey("unitTargetOverlay", (Keys) config.GetProperty<int>("unitTargetOverlay"));
            KeyManager.Instance.RegisterKey("moveCameraUp", (Keys)config.GetProperty<int>("moveCameraUp"));
            KeyManager.Instance.RegisterKey("moveCameraDown", (Keys)config.GetProperty<int>("moveCameraDown"));
            KeyManager.Instance.RegisterKey("moveCameraLeft", (Keys)config.GetProperty<int>("moveCameraLeft"));
            KeyManager.Instance.RegisterKey("moveCameraRight", (Keys)config.GetProperty<int>("moveCameraRight"));
        }

        /// <summary>
        /// Quits the game. Effectively closes the game.
        /// </summary>
        public static void Quit()
        {
            running = false;
        }

        public static Random Random => random;
        public static Point ScreenSize => new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
    }
}
