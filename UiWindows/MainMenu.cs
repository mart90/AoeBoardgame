using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace AoeBoardgame
{
    class MainMenu : IUiWindow
    {
        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }
        public int WidthPixels { get; set; }
        public int HeightPixels { get; set; }

        private TextNotification _textNotification;
        private string title;

        private readonly ServerHttpClient _httpClient;
        private readonly FontLibrary _fontLibrary;
        private readonly TextureLibrary _textureLibrary;

        public MainMenu(ServerHttpClient httpClient, FontLibrary fontLibrary, TextureLibrary textureLibrary)
        {
            CorrespondingUiState = UiState.MainMenu;
            WidthPixels = 500;
            HeightPixels = 500;
            title = "Untitled Boardgame";

            _httpClient = httpClient;
            _fontLibrary = fontLibrary;
            _textureLibrary = textureLibrary;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!WindowUtils.ApplicationIsActivated())
            {
                return;
            }

            ImGui.Begin("Menu");
            ImGui.SetWindowSize(new System.Numerics.Vector2(WidthPixels, HeightPixels + 60));
            ImGui.SetWindowPos(new System.Numerics.Vector2(0, -30));

            ImGui.PushFont(_fontLibrary.MedievalFont);
            ImGui.SetCursorPosY(100);
            ImGui.SetCursorPosX((WidthPixels - ImGui.CalcTextSize(title).X) * 0.5f);
            ImGui.Text(title);
            ImGui.PopFont();
            ImGui.Dummy(new System.Numerics.Vector2(500, 40));

            ImGui.SetCursorPosX((WidthPixels - 200) * 0.5f);
            if (ImGui.Button("Sandbox", new System.Numerics.Vector2(200, 50)))
            {
                NewUiState = UiState.Sandbox;
            }

            ImGui.SetCursorPosX((WidthPixels - 200) * 0.5f);
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

            ImGui.SetCursorPosX((WidthPixels - 200) * 0.5f);
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
