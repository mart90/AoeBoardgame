using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class LobbyBrowser : IUiWindow
    {
        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }
        public int WidthPixels { get; set; }
        public int HeightPixels { get; set; }

        public LobbyBrowser()
        {
            CorrespondingUiState = UiState.LobbyBrowser;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new System.NotImplementedException();
        }
    }
}
