using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGui;
using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class GameFrontEnd : Microsoft.Xna.Framework.Game
    {
        private ImGUIRenderer _guiRenderer;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Game _game;

        public GameFrontEnd()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = new TimeSpan(100000); // 100 updates p/s
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = 1020;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.ApplyChanges();
            _guiRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();

            base.Initialize();
        }

        protected override void LoadContent()
        {
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
            _game.StartTurn();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _game.SelectTileByLocation(mouseState.Position);
            }
            else if (mouseState.RightButton == ButtonState.Pressed)
            {
                _game.SetDestinationByLocation(mouseState.Position);
            }
            else
            {
                _game.HoverOverTileByLocation(mouseState.Position);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _guiRenderer.BeginLayout(gameTime);

            _game.Draw(_spriteBatch);

            _guiRenderer.EndLayout();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
