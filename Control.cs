using AoeBoardgame.Multiplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class Control : Microsoft.Xna.Framework.Game
    {
        private ImGUIRenderer _guiRenderer;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private List<IUiWindow> _uiWindows;
        private IUiWindow _activeWindow;

        private TextureLibrary _textureLibrary;
        private FontLibrary _fontLibrary;
        private ResearchLibrary _researchLibrary;
        private SoundEffectLibrary _soundEffectLibrary;

        private bool _leftMouseHeld;
        private bool _rightMouseHeld;

        private MultiplayerHttpClient _httpClient;

        public Control()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = new TimeSpan(100000); // 100 updates p/s
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _httpClient = new MultiplayerHttpClient();
            _guiRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _textureLibrary = new TextureLibrary(Content, _guiRenderer);
            _fontLibrary = new FontLibrary(Content);
            _researchLibrary = new ResearchLibrary();
            _soundEffectLibrary = new SoundEffectLibrary(Content);

            _uiWindows = new List<IUiWindow>
            {
                new MainMenu(),
                new LobbyBrowser(_textureLibrary, _fontLibrary, _researchLibrary, _soundEffectLibrary, _httpClient),
                new LoginScreen(_fontLibrary, _httpClient),
                new CreateLobbyForm(_fontLibrary, _httpClient)
            };

            ChangeUiWindow(_uiWindows.Single(e => e is MainMenu));
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (_activeWindow.NewUiState != null)
            {
                if (_activeWindow.NewUiState == UiState.Sandbox)
                {
                    _uiWindows.Add(new Sandbox(_textureLibrary, _fontLibrary, _researchLibrary, _soundEffectLibrary));
                }

                if (_activeWindow is LobbyBrowser browser && _activeWindow.NewUiState == UiState.MultiplayerGame)
                {
                    _uiWindows.Add(browser.CreatedGame);
                }

                if (_activeWindow.NewUiState == UiState.LoginScreen)
                {
                    if (_httpClient.AuthenticatedUser != null)
                    {
                        _activeWindow.NewUiState = UiState.LobbyBrowser;
                    }
                }

                var newWindow = GetUiWindowByState(_activeWindow.NewUiState.Value);
                _activeWindow.NewUiState = null;

                if (_activeWindow is CreateLobbyForm form && newWindow is LobbyBrowser)
                {
                    ((LobbyBrowser)newWindow).CreatedLobby = form.CreatedLobby;
                }

                if (_activeWindow is Game)
                {
                    _uiWindows.Remove(_activeWindow);
                    _activeWindow = null;
                }

                ChangeUiWindow(newWindow);
            }

            if (_activeWindow is Game game && IsActive)
            {
                MouseState mouseState = Mouse.GetState();

                if (mouseState.LeftButton == ButtonState.Released && _leftMouseHeld)
                {
                    _leftMouseHeld = false;
                }
                else if (mouseState.LeftButton == ButtonState.Pressed && !_leftMouseHeld)
                {
                    _leftMouseHeld = true;
                    game.LeftClickTileByLocation(mouseState.Position);
                }

                if (mouseState.RightButton == ButtonState.Released && _rightMouseHeld)
                {
                    _rightMouseHeld = false;
                }
                else if (mouseState.RightButton == ButtonState.Pressed && !_rightMouseHeld)
                {
                    _rightMouseHeld = true;
                    game.RightClickTileByLocation(mouseState.Position);
                }

                if (mouseState.RightButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Released)
                {
                    game.HoverOverTileByLocation(mouseState.Position);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _guiRenderer.BeginLayout(gameTime);

            _activeWindow.Update(_spriteBatch);

            _guiRenderer.EndLayout();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ChangeUiWindow(IUiWindow newWindow)
        {
            _activeWindow = newWindow;

            _graphics.PreferredBackBufferHeight = newWindow.HeightPixels;
            _graphics.PreferredBackBufferWidth = newWindow.WidthPixels;
            _graphics.ApplyChanges();
        }

        private IUiWindow GetUiWindowByState(UiState uiState)
        {
            return _uiWindows.Single(e => e.CorrespondingUiState == uiState);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (_activeWindow is LobbyBrowser browser && browser.CreatedLobby != null)
            {
                _httpClient.CancelLobby(browser.CreatedLobby.Id);
            }

            base.OnExiting(sender, args);
        }
    }
}
