using System.Collections.Generic;

namespace AoeBoardgame
{
    class Challenge : Game
    {
        public Challenge(TextureLibrary textureLibrary, FontLibrary fontLibrary, ResearchLibrary researchLibrary, SoundEffectLibrary soundEffectLibrary) 
            : base(textureLibrary, fontLibrary, researchLibrary, soundEffectLibrary)
        {
            CorrespondingUiState = UiState.Challenge;

            var mapGenerator = new MapGenerator(textureLibrary, 14);
            Map = mapGenerator.GenerateRandom(25, 21);

            Players = new List<Player>
            {
                new Player("Blue", new England(textureLibrary, researchLibrary), TileColor.Blue, TileColor.BlueUsed) 
                { 
                    IsActive = true,
                    IsLocalPlayer = true
                },
                new Player("Red", new France(textureLibrary, researchLibrary), TileColor.Red, TileColor.RedUsed)
                {
                    IsLocalPlayer = true
                }
            };

            PlaceStartingUnits();

            State = GameState.Default;

            StartTurn();
        }
    }
}
 