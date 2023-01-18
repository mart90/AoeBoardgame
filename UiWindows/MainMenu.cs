using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class MainMenu : IUiWindow
    {
        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }
        public int WidthPixels { get; set; }
        public int HeightPixels { get; set; }

        private TextNotification _textNotification;

        private readonly ServerHttpClient _httpClient;
        private readonly FontLibrary _fontLibrary;

        public MainMenu(ServerHttpClient httpClient, FontLibrary fontLibrary)
        {
            CorrespondingUiState = UiState.MainMenu;
            WidthPixels = 500;
            HeightPixels = 500;

            _httpClient = httpClient;
            _fontLibrary = fontLibrary;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!WindowUtils.ApplicationIsActivated())
            {
                return;
            }

            ImGui.Begin("Menu");

            ImGui.SetWindowFontScale(2f);
            ImGui.SetWindowSize(new System.Numerics.Vector2(WidthPixels, HeightPixels + 60));
            ImGui.SetWindowPos(new System.Numerics.Vector2(0, -30));

            if (ImGui.Button("Sandbox", new System.Numerics.Vector2(200, 50)))
            {
                NewUiState = UiState.Sandbox;
            }

            if (ImGui.Button("Multiplayer", new System.Numerics.Vector2(200, 50)))
            {
                if (_httpClient.RunningLatestVersion())
                {
                    NewUiState = UiState.LobbyBrowser;
                }
                else
                {
                    _textNotification = new TextNotification
                    {
                        FontColor = Color.Red,
                        Message = "Version mismatch. Get the latest from the discord"
                    };
                }
            }

            if (ImGui.Button("Challenges", new System.Numerics.Vector2(200, 50)))
            {
                NewUiState = UiState.ChallengeBrowser;
            }

            ImGui.End();

            if (_textNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, _textNotification.Message, new Vector2(20, HeightPixels - 60), _textNotification.FontColor);
            }
        }
    }
}
