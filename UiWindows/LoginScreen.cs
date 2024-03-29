﻿using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class LoginScreen : IUiWindow
    {
        public UiState CorrespondingUiState { get; set; }
        public int WidthPixels { get; set; }
        public int HeightPixels { get; set; }
        public UiState? NewUiState { get; set; }

        private readonly byte[] _usernameBuffer;
        private readonly byte[] _passwordBuffer;

        private TextNotification _textNotification;

        private readonly FontLibrary _fontLibrary;
        private readonly ServerHttpClient _httpClient;

        public LoginScreen(FontLibrary fontLibrary, ServerHttpClient httpClient)
        {
            CorrespondingUiState = UiState.LoginScreen;
            WidthPixels = 500;
            HeightPixels = 500;

            _httpClient = httpClient;
            _fontLibrary = fontLibrary;

            _usernameBuffer = new byte[100];
            _passwordBuffer = new byte[100];
        }

        public void TryRegister()
        {
            string username = _usernameBuffer.GetString();
            string plainTextPassword = _passwordBuffer.GetString();

            if (username == "" || plainTextPassword == "")
            {
                return;
            }

            if (username.Length > 12)
            {
                _textNotification = new TextNotification
                {
                    FontColor = Color.Red,
                    Message = "Username too long. Max 12 characters"
                };
                return;
            }

            var existingUser = _httpClient.GetUserIdByName(username);

            if (existingUser != null)
            {
                _textNotification = new TextNotification
                {
                    FontColor = Color.Red,
                    Message = "Username is taken"
                };
                return;
            }

            _httpClient.RegisterUser(username, plainTextPassword);

            NewUiState = UiState.MainMenu;
        }

        public void TryLogin()
        {
            bool loginSuccess = _httpClient.Login(_usernameBuffer.GetString(), _passwordBuffer.GetString());

            if (loginSuccess)
            {
                NewUiState = UiState.MainMenu;
            }
            else
            {
                _textNotification = new TextNotification
                {
                    FontColor = Color.Red,
                    Message = "Login failed"
                };
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!WindowUtils.ApplicationIsActivated())
            {
                return;
            }

            ImGui.Begin("LoginScreen");

            ImGui.SetWindowFontScale(2f);
            ImGui.SetWindowSize(new System.Numerics.Vector2(WidthPixels, HeightPixels + 60));
            ImGui.SetWindowPos(new System.Numerics.Vector2(0, -30));

            ImGui.InputText("Username", _usernameBuffer, (uint)_usernameBuffer.Length);
            ImGui.InputText("Password", _passwordBuffer, (uint)_passwordBuffer.Length);

            if (ImGui.Button("Login"))
            {
                TryLogin();
            }

            if (ImGui.Button("Register"))
            {
                TryRegister();
            }

            ImGui.End();

            if (_textNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, _textNotification.Message, new Vector2(20, HeightPixels - 60), _textNotification.FontColor);
            }
        }
    }
}
