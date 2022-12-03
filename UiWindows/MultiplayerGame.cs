using System.Collections.Generic;

namespace AoeBoardgame
{
    class MultiplayerGame : Game, IUiWindow
    {
        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }

        public MultiplayerGame(
            List<Player> players, 
            Map map, 
            TextureLibrary textureLibrary, 
            FontLibrary fontLibrary, 
            ResearchLibrary researchLibrary) : base(textureLibrary, fontLibrary, researchLibrary)
        {
            CorrespondingUiState = UiState.MultiplayerGame;
        }
    }
}
