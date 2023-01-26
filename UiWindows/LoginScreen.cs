using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

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
        private readonly SoundEffectLibrary _soundEffectLibrary;
        private readonly TextureLibrary _textureLibrary;
        private readonly ServerHttpClient _httpClient;

        public LoginScreen(FontLibrary fontLibrary, ServerHttpClient httpClient, SoundEffectLibrary soundEffectLibrary, TextureLibrary textureLibrary)
        {
            CorrespondingUiState = UiState.LoginScreen;
            WidthPixels = 505;
            HeightPixels = 500;

            _httpClient = httpClient;
            _fontLibrary = fontLibrary;
            _textureLibrary = textureLibrary;
            _soundEffectLibrary = soundEffectLibrary;

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

            ImGui.Begin("LoginScreen", ImGuiWindowFlags.NoBackground|ImGuiWindowFlags.NoMove|ImGuiWindowFlags.NoTitleBar|ImGuiWindowFlags.NoResize);
            ImGui.PushFont(_fontLibrary.MedievalFont);
            ImGui.SetWindowSize(new System.Numerics.Vector2(WidthPixels, HeightPixels));
            ImGui.SetWindowPos(new System.Numerics.Vector2(0, 0));
            ImGui.GetWindowDrawList().AddImage(_textureLibrary.GetIntPtrByUiType(UiType.LoginScreenBackground), new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(WidthPixels, HeightPixels));

            // Logo
            ImGui.SetCursorPosX((WidthPixels - 344) * 0.5f);
            ImGui.SetCursorPosY(20);
            ImGui.Image(_textureLibrary.GetIntPtrByUiType(UiType.Logo), new System.Numerics.Vector2(344, 46));

            ImGui.SetCursorPosX((WidthPixels - 200) * 0.5f);
            ImGui.SetCursorPosY(20);
            ImGui.Image(_textureLibrary.GetIntPtrByUiType(UiType.Shields), new System.Numerics.Vector2(265, 269));

            // Username input
            float inputWidth = 300f;
            float inputHeight = 33f;
            float locationX = 100;
            float locationY = 250;
            ImGui.GetWindowDrawList().AddImage(_textureLibrary.GetIntPtrByUiType(UiType.InputText), new System.Numerics.Vector2(locationX, locationY), new System.Numerics.Vector2(locationX + inputWidth, locationY + inputHeight));
            ImGui.SetCursorScreenPos(new System.Numerics.Vector2(locationX, locationY));
            ImGui.SetNextItemWidth(inputWidth);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.8f, 0.7f, 0.5f, 1));
            ImGui.InputText("##username", _usernameBuffer, (uint)_usernameBuffer.Length, ImGuiInputTextFlags.AutoSelectAll);
            ImGui.PopStyleColor(2);

            // Password input
            locationY += 40;
            ImGui.GetWindowDrawList().AddImage(_textureLibrary.GetIntPtrByUiType(UiType.InputText), new System.Numerics.Vector2(locationX, locationY), new System.Numerics.Vector2(locationX + inputWidth, locationY + inputHeight));
            ImGui.SetCursorScreenPos(new System.Numerics.Vector2(locationX, locationY));
            ImGui.SetNextItemWidth(inputWidth);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0, 0, 0, 0));
            ImGui.InputText("##password", _passwordBuffer, (uint)_passwordBuffer.Length, ImGuiInputTextFlags.Password);
            ImGui.PopStyleColor();

            // Login button
            UiButton.Create(
                _textureLibrary,
                UiType.LoginButton,
                new System.Numerics.Vector2(locationX, locationY +40), 
                () => {
                    _soundEffectLibrary.ButtonClick.Play();
                    TryLogin();
                }
            );

            // Register button
            UiButton.Create(
                _textureLibrary,
                UiType.RegisterButton,
                new System.Numerics.Vector2(locationX, locationY +40),
                () => {
                    _soundEffectLibrary.ButtonClick.Play();
                    TryRegister();
                }
            );

            ImGui.PopFont();
            ImGui.End();

            if (_textNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, _textNotification.Message, new Vector2(20, HeightPixels - 60), _textNotification.FontColor);
            }
        }
    }
}
