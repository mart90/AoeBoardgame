using AoeBoardgame.Multiplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;

namespace AoeBoardgame
{
    class LobbyBrowser : IUiWindow
    {
        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }
        public int WidthPixels { get; set; }
        public int HeightPixels { get; set; }

        public List<Lobby> Lobbies { get; private set; }
        public Lobby CreatedLobby { get; set; }
        public DateTime LastRefresh { get; set; }

        public TextNotification TextNotification { get; private set; }

        private readonly FontLibrary _fontLibrary;
        private readonly TextureLibrary _textureLibrary;
        private readonly ResearchLibrary _researchLibrary;
        private readonly MultiplayerHttpClient _httpClient;

        public MultiplayerGame CreatedGame { get; private set; }

        public LobbyBrowser(
            TextureLibrary textureLibrary,
            FontLibrary fontLibrary,
            ResearchLibrary researchLibrary,
            MultiplayerHttpClient httpClient)
        {
            CorrespondingUiState = UiState.LobbyBrowser;
            WidthPixels = 800;
            HeightPixels = 500;

            _httpClient = httpClient;
            _fontLibrary = fontLibrary;
            _textureLibrary = textureLibrary;
            _researchLibrary = researchLibrary;

            Lobbies = new List<Lobby>();
        }

        private void Refresh()
        {
            Lobby selectedLobby = Lobbies.SingleOrDefault(e => e.IsSelected);

            Lobbies = _httpClient.GetActiveLobbies();

            if (selectedLobby != null)
            {
                Lobby selectedLobbyNew = Lobbies.SingleOrDefault(e => e.Id == selectedLobby.Id);

                if (selectedLobbyNew != null)
                {
                    selectedLobbyNew.IsSelected = true;
                }
            }

            LastRefresh = DateTime.Now;
        }

        private void CancelLobby()
        {
            _httpClient.CancelLobby(CreatedLobby.Id);
            CreatedLobby = null;
        }

        public bool PlayerJoinedMyLobby()
        {
            return CreatedLobby != null && !Lobbies.Any(e => e.Id == CreatedLobby.Id);
        }

        public void HandlePlayerJoined()
        {
            if (CreatedLobby.Settings.MapSeed == null)
            {
                // Joining player generated the map. Get the seed from server
                CreatedLobby.Settings.MapSeed = _httpClient.GetGeneratedMapSeed(CreatedLobby.Id);
            }

            CreatedGame = new MultiplayerGame(CreatedLobby.Settings, _textureLibrary, _fontLibrary, _researchLibrary, _httpClient)
            {
                Id = _httpClient.GetGameId(CreatedLobby.Id)
            };

            CreatedGame.SetLocalPlayer(CreatedLobby.Settings.MapSeed[0] == 'b');

            CreatedLobby.Settings.MapSeed = null; // Also sets it to null on the create lobby form (for next game)

            Thread.Sleep(1000);

            CreatedGame.SetOpponent();

            NewUiState = UiState.MultiplayerGame;

            CreatedLobby = null;
        }

        private void JoinLobby(Lobby lobby)
        {
            CreatedGame = new MultiplayerGame(lobby.Settings, _textureLibrary, _fontLibrary, _researchLibrary, _httpClient);

            lobby.Settings.MapSeed = CreatedGame.MapSeed;

            CreatedGame.Id = _httpClient.JoinLobby(lobby);

            CreatedGame.SetLocalPlayer(lobby.Settings.MapSeed[0] != 'b');
            CreatedGame.SetOpponent();

            NewUiState = UiState.MultiplayerGame;
        }

        private void CreateLobby()
        {
            var settings = new MultiplayerGameSettings();
            CreatedLobby = new Lobby 
            { 
                Id = _httpClient.CreateLobby(settings),
                Settings = settings
            };
        }

        public void Update(SpriteBatch spriteBatch)
        {
            if ((DateTime.Now - LastRefresh).TotalSeconds > 5)
            {
                Refresh();

                if (CreatedLobby != null && PlayerJoinedMyLobby())
                {
                    HandlePlayerJoined();
                    return;
                }
            }

            if (!WindowUtils.ApplicationIsActivated())
            {
                return;
            }

            ImGui.Begin("LobbyBrowser");

            ImGui.SetWindowFontScale(2f);
            ImGui.SetWindowSize(new System.Numerics.Vector2(WidthPixels, HeightPixels + 60));
            ImGui.SetWindowPos(new System.Numerics.Vector2(0, -30));

            ImGui.BeginChild("Lobbies", new System.Numerics.Vector2(800, 400));

            for (int rowNumber = 0; rowNumber < Lobbies.Count; rowNumber++)
            {
                Lobby lobby = Lobbies[rowNumber];

                if (ImGui.Button("Join"))
                {
                    JoinLobby(lobby);
                }

                ImGui.PushID(rowNumber);

                ImGui.SameLine();
                ImGui.Text(lobby.Title());
            }

            ImGui.EndChild();

            if (ImGui.Button("Back"))
            {
                NewUiState = UiState.MainMenu;
            }

            ImGui.SameLine();

            if (CreatedLobby == null)
            {
                if (ImGui.Button("Create"))
                {
                    CreateLobby();
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
