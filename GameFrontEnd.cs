﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AoeBoardgame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class GameFrontEnd : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Game _game;

        public GameFrontEnd()
        {
            _graphics = new GraphicsDeviceManager(this);
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
            _graphics.PreferredBackBufferHeight = 1020;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.ApplyChanges();

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
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var textureLibrary = new TextureLibrary(Content);
            var researchLibrary = new ResearchLibrary();

            var mapGenerator = new MapGenerator(textureLibrary, 14);    
            Map gameMap = mapGenerator.GenerateMap(25, 22);
            var players = new List<Player>
            {
                new Player(new Britons(textureLibrary), TileColor.Blue, researchLibrary),
                new Player(new Britons(textureLibrary), TileColor.Red, researchLibrary)
            };

            _game = new Game(players, gameMap);
            _game.PlaceStartingTownCenters();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _game.SelectTileByLocation(mouseState.Position);
            }
            else
            {
                _game.HoverOverTileByLocation(mouseState.Position);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself like one of its French girls
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _game.DrawMap(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
