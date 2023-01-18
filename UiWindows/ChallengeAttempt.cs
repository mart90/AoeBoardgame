using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class ChallengeAttempt : Game
    {
        public Challenge Challenge { get; set; }

        public ChallengeAttempt(TextureLibrary textureLibrary, FontLibrary fontLibrary, ResearchLibrary researchLibrary, SoundEffectLibrary soundEffectLibrary, Challenge challenge) 
            : base(textureLibrary, fontLibrary, researchLibrary, soundEffectLibrary)
        {
            CorrespondingUiState = UiState.ChallengeAttempt;
            Challenge = challenge;

            var mapGenerator = new MapGenerator(textureLibrary, 14);
            Map = mapGenerator.GenerateFromSeed(Challenge.MapSeed);

            Players = new List<Player>
            {
                new Player("Blue", new England(textureLibrary, researchLibrary), TileColor.Blue, TileColor.BlueUsed)
                { 
                    IsActive = true,
                    IsLocalPlayer = true
                }
            };

            if (Challenge.ChallengeType == ChallengeType.EarlyRush)
            {
                Players.Add(new Player("Red", new France(textureLibrary, researchLibrary), TileColor.Red, TileColor.RedUsed));
            }

            PlaceStartingUnits();

            State = GameState.Default;

            StartTurn();

            Popup = new Popup
            {
                Message = $"{Challenge.Description}\n\nBronze: {Challenge.BronzeTurns} turns\nSilver: {Challenge.SilverTurns} turns\nGold: {Challenge.GoldTurns} turns",
                IsInformational = true
            };
        }

        protected override void PlaceStartingUnits()
        {
            Map.Tiles[255].SetObject(Players[0].AddAndGetPlaceableObject(typeof(TownCenter)));

            if (Challenge.ChallengeType == ChallengeType.EarlyRush)
            {
                Map.Tiles[270].SetObject(Players[1].AddAndGetPlaceableObject(typeof(TownCenter)));
            }

            Map.Tiles[254].SetObject(Players[0].AddAndGetPlaceableObject(typeof(Villager)));
            Map.Tiles[256].SetObject(Players[0].AddAndGetPlaceableObject(typeof(Villager)));
            Map.Tiles[280].SetObject(Players[0].AddAndGetPlaceableObject(typeof(Villager)));

            Map.Tiles[230].SetObject(Players[0].AddAndGetPlaceableObject(typeof(Scout)));

            IEnumerable<PlayerObject> startingUnits = Players.SelectMany(e => e.OwnedObjects);

            foreach (Scout scout in startingUnits.Where(e => e is Scout))
            {
                scout.AttackDamage = 0;
            }

            foreach (PlayerObject obj in startingUnits)
            {
                UpdateVisibleAndRangeableTilesForObject(obj);
            }
        }

        protected override void EndTurn()
        {
            CheckChallengeCompleted();

            ClearCurrentSelection();

            MoversTakeSteps();

            // TODO remove, shouldn't be needed
            DestroyEmptyArmies();

            ActivePlayer.ResetMovement();
            ActivePlayer.ResetResourcesGatheredLastTurn();

            GatherResources();
            ConsumeGold();

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                IsEndOfTurn = true
            });

            StartTurn();
        }

        private void CheckChallengeCompleted()
        {
            int turnCount = ActivePlayerTurnCount();

            if (Challenge.ChallengeType == ChallengeType.EarlyRush)
            {
                if (InActivePlayer.OwnedObjects.Any())
                {
                    return;
                }
            }
            else if (Challenge.ChallengeType == ChallengeType.WonderRush)
            {
                if (!ActivePlayer.OwnedObjects.Any(e => e is Wonder))
                {
                    return;
                }
            }
            else if (Challenge.ChallengeType == ChallengeType.Boom)
            {
                if (ActivePlayer.OwnedObjects.Count(e => e is Villager) < 50)
                {
                    return;
                }
            }

            string medal = null;
            if (turnCount <= Challenge.GoldTurns)
            {
                medal = "gold";
            }
            else if (turnCount <= Challenge.SilverTurns)
            {
                medal = "silver";
            }
            else if (turnCount <= Challenge.BronzeTurns)
            {
                medal = "bronze";
            }

            string message = $"You have completed the challenge in {ActivePlayerTurnCount()} turns.";

            if (medal != null)
            {
                message += $"You've won the {medal} medal!";
            }

            RevealMap();

            Popup = new Popup
            {
                Message = message,
                IsInformational = true
            };

            // TODO upload result
        }

        protected override void CheckForMilitaryVictory() { }
    }
}
 