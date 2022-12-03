using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class MainMenu : IUiWindow
    {
        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }
        public int WidthPixels { get; set; }
        public int HeightPixels { get; set; }

        public MainMenu()
        {
            CorrespondingUiState = UiState.MainMenu;
            WidthPixels = 500;
            HeightPixels = 500;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ImGui.Begin("Menu");

            ImGui.SetWindowFontScale(2f);
            ImGui.SetWindowSize(new System.Numerics.Vector2(WidthPixels, HeightPixels + 60));
            ImGui.SetWindowPos(new System.Numerics.Vector2(0, -30));

            if (ImGui.Button("Sandbox", new System.Numerics.Vector2(150, 50)))
            {
                NewUiState = UiState.Sandbox;
            }

            ImGui.End();
        }
    }
}
