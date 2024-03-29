﻿using Microsoft.Xna.Framework.Graphics;
using System;
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

            IsTimeControlEnabled = true;
            LastEndTurnTimestamp = DateTime.Now;
            TimeIncrementSeconds = 0;
            StartTimeMinutes = 600;

            Players = new List<Player>
            {
                new Player("Blue", new England(textureLibrary, researchLibrary), TileColor.Blue, TileColor.BlueUsed)
                {
                    IsActive = true,
                    IsLocalPlayer = true,
                    TimeMiliseconds = StartTimeMinutes.Value * 60000
                },
                new Player("Red", new France(textureLibrary, researchLibrary), TileColor.Red, TileColor.RedUsed)
                {
                    IsLocalPlayer = true,
                    TimeMiliseconds = StartTimeMinutes.Value * 60000
                }
            };

            PlaceStartingUnits();
            State = GameState.Default;

            StartTurn();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
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
 