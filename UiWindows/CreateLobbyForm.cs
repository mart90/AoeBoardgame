using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private readonly byte[] _startTimeMinutesBuffer;
        private readonly byte[] _timeIncrementSecondsBuffer;

        private bool _hostPlaysBlue;
        private bool _restoreGame;
        private bool _isTimeControlEnabled;

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

            _startTimeMinutesBuffer = new byte[10];
            _timeIncrementSecondsBuffer = new byte[10];

            Encoding.ASCII.GetBytes("10").CopyTo(_startTimeMinutesBuffer, 0);
            Encoding.ASCII.GetBytes("30").CopyTo(_timeIncrementSecondsBuffer, 0);

            _httpClient = httpClient;
            _fontLibrary = fontLibrary;

            _hostPlaysBlue = true;
        }
        
        private bool CreateLobby()
        {
            var settings = new MultiplayerGameSettings();

            if (_isTimeControlEnabled)
            {
                settings.IsTimeControlEnabled = true;

                string startTimeMinutesStr = _startTimeMinutesBuffer.GetString();
                if (int.TryParse(startTimeMinutesStr, out int startTimeMinutes))
                {
                    settings.StartTimeMinutes = startTimeMinutes;
                }
                else
                {
                    TextNotification = new TextNotification
                    {
                        FontColor = Color.Red,
                        Message = "Start time must be a number"
                    };

                    return false;
                }

                string timeIncrementSecondsStr = _timeIncrementSecondsBuffer.GetString();
                if (int.TryParse(timeIncrementSecondsStr, out int timeIncrementSeconds))
                {
                    settings.TimeIncrementSeconds = timeIncrementSeconds;
                }
                else
                {
                    TextNotification = new TextNotification
                    {
                        FontColor = Color.Red,
                        Message = "Time increment must be a number"
                    };

                    return false;
                }
            }

            if (_restoreGame)
            {
                if (int.TryParse(_restoreGameIdBuffer.GetString(), out int restoreGameId))
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

                    return false;
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

                        return false;
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

            return true;
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

            ImGui.Checkbox("Time control", ref _isTimeControlEnabled);

            if (_isTimeControlEnabled)
            {
                ImGui.SetNextItemWidth(70);
                ImGui.InputText("Main time (minutes)", _startTimeMinutesBuffer, (uint)_startTimeMinutesBuffer.Length);

                ImGui.SetNextItemWidth(70);
                ImGui.InputText("Time increment (seconds)", _timeIncrementSecondsBuffer, (uint)_timeIncrementSecondsBuffer.Length);
            }

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
                    if (CreateLobby())
                    {
                        NewUiState = UiState.LobbyBrowser;
                    }
                }
            }

            ImGui.End();

            if (TextNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, TextNotification.Message, new Vector2(10, HeightPixels - 40), TextNotification.FontColor);
            }
        }
    }
}
