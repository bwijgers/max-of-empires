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
        public static Camera camera;
        public static Vector2 overlayPos;
        public static GraphicsDeviceManager graphics;
        public static InputHelper inputHelper;
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private static bool running = true;

        private SpriteBatch gameObjectSpriteBatch;
        private SpriteBatch overlaySpriteBatch;
        private Configuration mainConfiguration;
        private Vector2 windowSize = new Vector2(1280, 768);

        /// <summary>
        /// 800 * 480 = 15 * 15
        /// 1280 * 768 = 24 * 24
        /// 1920 * 1080 = 34 * 34
        /// </summary>
        public static Point minGridSize = new Point(24, 24);

        /// <summary>
        /// The fullscreen scaling matrix
        /// </summary>
        private Matrix spriteScale = Matrix.CreateScale(1, 1, 1);

        public MaxOfEmpires()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = (int)windowSize.X;
            graphics.PreferredBackBufferHeight = (int)windowSize.Y;
            overlayPos = new Vector2((graphics.PreferredBackBufferWidth - (graphics.PreferredBackBufferWidth - graphics.PreferredBackBufferHeight)), 0);

            camera = new Camera();
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
            IsMouseVisible = true;

            base.Initialize();
        }

        protected void LoadConfiguration()
        {
            // Initialize main configuration file
            mainConfiguration = FileManager.LoadConfig("Main");

            // Initialize units
            Configuration unitConfiguration = mainConfiguration.GetPropertySection("unit");
            SoldierRegistry.Init(unitConfiguration);

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
            gameObjectSpriteBatch = new SpriteBatch(GraphicsDevice);
            overlaySpriteBatch = new SpriteBatch(GraphicsDevice);

            // Init AssetManager and DrawingHelper
            AssetManager.Init(Content);
            DrawingHelper.Init(GraphicsDevice);

            // Load the main configuration file, load all subconfigs
            LoadConfiguration();

            // Adds battleState to the GamestateManager
            GameStateManager.AddState("economy", new EconomyState());
            GameStateManager.AddState("battle", new BattleState());
            GameStateManager.AddState("mainMenu", new MainMenuState());
            GameStateManager.SwitchState("mainMenu", true);
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
                camera.UseMouse = !camera.UseMouse;
            }
            //EINDE TIJDELIJKE CODE

            //TIJDELIJKE CODE OMDAT IK GEEN IDEE HEB WAAR IK DIT ANDERS ZOU KUNNEN ZETTEN
            if (inputHelper.KeyPressed(Keys.F11))
            {
                ToggleFullScreen();
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
            Matrix transform = Matrix.CreateScale(new Vector3(camera.Zoom, camera.Zoom, 1)); // Zoom default 1

            overlaySpriteBatch.Begin();
            gameObjectSpriteBatch.Begin( // SpriteBatch variable
                        SpriteSortMode.Deferred, // Sprite sort mode - not related
                        BlendState.NonPremultiplied, // BelndState - not related
                        null,
                        null,
                        null,
                        null,
                        transform * SpriteScale); // set camera tranformation

            GameStateManager.Draw(gameTime, gameObjectSpriteBatch, overlaySpriteBatch);// Draw the current game state

            gameObjectSpriteBatch.End();
            overlaySpriteBatch.End();

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

            KeyManager.Instance.RegisterKey("zoomCameraIn", Keys.PageUp);
            KeyManager.Instance.RegisterKey("zoomCameraOut", Keys.PageDown);
        }

        /// <summary>
        /// Quits the game. Effectively closes the game.
        /// </summary>
        public static void Quit()
        {
            running = false;
        }

        private void ToggleFullScreen()
        {
            if (!graphics.IsFullScreen)
            {
                graphics.IsFullScreen = true;

                inputHelper.DisplayScale = new Vector2((float)graphics.PreferredBackBufferWidth / GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, (float)graphics.PreferredBackBufferHeight / GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            }
            else
            {
                graphics.IsFullScreen = false;

                inputHelper.DisplayScale = new Vector2(graphics.PreferredBackBufferWidth / windowSize.X, graphics.PreferredBackBufferHeight / windowSize.Y);
            }

            graphics.ApplyChanges();
        }

        /// <summary>
        /// The fullscreen scaling matrix
        /// </summary>
        public Matrix SpriteScale
        {
            get
            {
                return spriteScale;
            }
            set
            {
                spriteScale = value;
            }
        }

        public static Vector2 OverlayPos => overlayPos;
        public static Random Random => random;
        public static Point ScreenSize => new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
    }
}
