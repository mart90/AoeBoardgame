using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Sandbox : Game, IUiWindow
    {
        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }

        public Sandbox(TextureLibrary textureLibrary, FontLibrary fontLibrary, ResearchLibrary researchLibrary) 
            : base(textureLibrary, fontLibrary, researchLibrary)
        {
            CorrespondingUiState = UiState.Sandbox;

            var mapGenerator = new MapGenerator(textureLibrary, 14);
            Map = mapGenerator.GenerateRandom(25, 21);

            Players = new List<Player>
            {
                new Player("Blue", new England(textureLibrary, researchLibrary), TileColor.Blue) 
                { 
                    IsActive = true,
                    IsLocalPlayer = true
                },
                new Player("Red", new France(textureLibrary, researchLibrary), TileColor.Red)
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
    }
}
 