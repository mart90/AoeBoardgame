using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    interface IUiWindow
    {
        UiState CorrespondingUiState { get; set; }
        int WidthPixels { get; set; }
        int HeightPixels { get; set; }
        UiState? NewUiState { get; set; }
        void Update(SpriteBatch spriteBatch);
    }
}
