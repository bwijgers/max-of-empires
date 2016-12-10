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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private InputHelper inputHelper;
        private static Random random = new Random((int) DateTime.Now.Ticks);

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
            GameStateManager.AddState("battleState", new BattleState());
            GameStateManager.SwitchState("battleState");

            // Initialize the key inputs
            InitializeKeys();

            // Init the battle grid
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
            inputHelper.Update(gameTime);

            // Update current gamestate
            GameStateManager.Update(gameTime);
            GameStateManager.HandleInput(inputHelper, KeyManager.Instance);

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

        }

        public static Random Random => random;
    }
}
