using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace AoeBoardgame
{
    class CreateLobbyForm : IUiWindow
    {
        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }
        public int WidthPixels { get; set; }
        public int HeightPixels { get; set; }

        public TextNotification TextNotification { get; private set; }

        private readonly FontLibrary _fontLibrary;
        private readonly ServerHttpClient _httpClient;

        private readonly byte[] _restoreGameIdBuffer;
        private readonly byte[] _restoreMoveNumberBuffer;

        private bool _hostPlaysBlue;
        private bool _restoreGame;

        public Lobby CreatedLobby { get; set; }

        public CreateLobbyForm(
            FontLibrary fontLibrary,
            ServerHttpClient httpClient)
        {
            CorrespondingUiState = UiState.CreatingLobby;
            WidthPixels = 800;
            HeightPixels = 500;

            _restoreGameIdBuffer = new byte[10];
            _restoreMoveNumberBuffer = new byte[10];

            _httpClient = httpClient;
            _fontLibrary = fontLibrary;

            _hostPlaysBlue = true;
        }
        
        private void CreateLobby()
        {
            var settings = new MultiplayerGameSettings();

            string restoreGameIdStr = _restoreGameIdBuffer.GetString();

            if (restoreGameIdStr != "")
            {
                if (int.TryParse(restoreGameIdStr, out int restoreGameId))
                {
                    settings.RestoreGameId = restoreGameId;
                }
                else
                {
                    TextNotification = new TextNotification
                    {
                        FontColor = Color.Red,
                        Message = "Game id to restore must be a number"
                    };

                    return;
                }

                string restoreGameMoveNumberStr = _restoreMoveNumberBuffer.GetString();

                if (restoreGameMoveNumberStr == "")
                {
                    settings.RestoreMoveNumber = null;
                }
                else
                {
                    if (int.TryParse(restoreGameMoveNumberStr, out int restoreGameMoveNumber))
                    {
                        settings.RestoreMoveNumber = restoreGameMoveNumber;
                    }
                    else
                    {
                        TextNotification = new TextNotification
                        {
                            FontColor = Color.Red,
                            Message = "Move number to restore to must be a number"
                        };

                        return;
                    }
                }

                settings.MapSeed = _httpClient.GetMapSeedByGameId(restoreGameId);
            }

            settings.HostPlaysBlue = _hostPlaysBlue;

            CreatedLobby = new Lobby 
            { 
                Id = _httpClient.CreateLobby(settings),
                Settings = settings
            };
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!WindowUtils.ApplicationIsActivated())
            {
                return;
            }

            ImGui.Begin("Create lobby");

            ImGui.SetWindowFontScale(2f);
            ImGui.SetWindowSize(new System.Numerics.Vector2(WidthPixels, HeightPixels + 60));
            ImGui.SetWindowPos(new System.Numerics.Vector2(0, -30));

            ImGui.BeginChild("Form", new System.Numerics.Vector2(800, 400));

            ImGui.Checkbox("Host plays England", ref _hostPlaysBlue);

            ImGui.NewLine();
            
            ImGui.Checkbox("Restore game", ref _restoreGame);

            if (_restoreGame)
            {
                ImGui.SetNextItemWidth(70);
                ImGui.InputText("Game id", _restoreGameIdBuffer, (uint)_restoreGameIdBuffer.Length);

                ImGui.SameLine();

                if (ImGui.Button("My last game"))
                {
                    string game_id = _httpClient.GetMyLastGameId();

                    if (game_id != null)
                    {
                        Encoding.ASCII.GetBytes(game_id).CopyTo(_restoreGameIdBuffer, 0);
                    }
                }

                ImGui.SetNextItemWidth(70);
                ImGui.InputText("To move number", _restoreMoveNumberBuffer, (uint)_restoreMoveNumberBuffer.Length);
            }

            ImGui.EndChild();

            if (ImGui.Button("Back"))
            {
                NewUiState = UiState.LobbyBrowser;
            }

            ImGui.SameLine();

            if (CreatedLobby == null)
            {
                if (ImGui.Button("Create"))
                {
                    CreateLobby();
                    NewUiState = UiState.LobbyBrowser;
                }
            }

            ImGui.End();

            if (TextNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, TextNotification.Message, new Vector2(20, HeightPixels - 60), TextNotification.FontColor);
            }
        }
    }
}
