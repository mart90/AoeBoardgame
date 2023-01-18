using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class ChallengeBrowser : IUiWindow
    {
        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }
        public int WidthPixels { get; set; }
        public int HeightPixels { get; set; }

        public List<Challenge> PossibleChallenges { get; set; }
        public Challenge ChosenChallenge { get; set; }

        private TextNotification _textNotification;

        private readonly ServerHttpClient _httpClient;
        private readonly FontLibrary _fontLibrary;

        public ChallengeBrowser(ServerHttpClient httpClient, FontLibrary fontLibrary)
        {
            CorrespondingUiState = UiState.ChallengeBrowser;
            WidthPixels = 500;
            HeightPixels = 500;

            _httpClient = httpClient;
            _fontLibrary = fontLibrary;

            PossibleChallenges = _httpClient.GetChallenges();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!WindowUtils.ApplicationIsActivated())
            {
                return;
            }

            ImGui.Begin("ChallengeBrowser");

            ImGui.SetWindowSize(new System.Numerics.Vector2(WidthPixels, HeightPixels + 60));
            ImGui.SetWindowPos(new System.Numerics.Vector2(0, -20));

            foreach (var challenge in PossibleChallenges)
            {
                if (ImGui.Button(challenge.UiName, new System.Numerics.Vector2(200, 50)))
                {
                    ChosenChallenge = challenge;
                    NewUiState = UiState.ChallengeAttempt;
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(challenge.Description);
                }
            }

            ImGui.Dummy(new System.Numerics.Vector2(1, 400 - PossibleChallenges.Count * 55));

            if (ImGui.Button("Back", new System.Numerics.Vector2(200, 50)))
            {
                NewUiState = UiState.MainMenu;
            }

            ImGui.End();

            if (_textNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, _textNotification.Message, new Vector2(20, HeightPixels - 60), _textNotification.FontColor);
            }
        }
    }
}
