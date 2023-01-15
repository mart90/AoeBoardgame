using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Sandbox : Game
    {
        public Sandbox(TextureLibrary textureLibrary, FontLibrary fontLibrary, ResearchLibrary researchLibrary, SoundEffectLibrary soundEffectLibrary) 
            : base(textureLibrary, fontLibrary, researchLibrary, soundEffectLibrary)
        {
            CorrespondingUiState = UiState.Sandbox;

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

        public override void Update(SpriteBatch spriteBatch)
        {
            base.Update(spriteBatch);
        }

        public override void StartTurn()
        {
            SetFogOfWar(ActivePlayer);
            base.StartTurn();
        }

        protected override void EndGame()
        {
            base.EndGame();

            Popup = new Popup
            {
                IsInformational = true,
                Message = "The game has ended"
            };
        }
    }
}
 