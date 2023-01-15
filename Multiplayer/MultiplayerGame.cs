using AoeBoardgame.Multiplayer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class MultiplayerGame : Game
    {
        public int Id { get; set; }

        private readonly User _us;
        private User _opponent;
        private bool _restoringGame;

        private readonly MultiplayerHttpClient _httpClient;

        private DateTime _lastPoll;

        public string MapSeed => Map.Seed;

        private Player _localPlayer => Players.Single(e => e.IsLocalPlayer);
        protected override Player VisiblePlayer => _localPlayer;

        public MultiplayerGame(
            MultiplayerGameSettings settings,
            TextureLibrary textureLibrary, 
            FontLibrary fontLibrary, 
            ResearchLibrary researchLibrary,
            SoundEffectLibrary soundEffectLibrary,
            MultiplayerHttpClient httpClient) : base(textureLibrary, fontLibrary, researchLibrary, soundEffectLibrary)
        {
            CorrespondingUiState = UiState.MultiplayerGame;
            _httpClient = httpClient;

            _us = _httpClient.AuthenticatedUser;

            Players = new List<Player>()
            {
                new Player("Blue", new England(textureLibrary, researchLibrary), TileColor.Blue, TileColor.BlueUsed)
                {
                    IsActive = true
                },
                new Player("Red", new France(textureLibrary, researchLibrary), TileColor.Red, TileColor.RedUsed)
            };

            MapGenerator mapGenerator = new MapGenerator(textureLibrary, 14);

            if (settings.MapSeed != null)
            {
                Map = mapGenerator.GenerateFromSeed(settings.MapSeed);
            }
            else
            {
                // Joining player generates the map
                Map = mapGenerator.GenerateRandom(25, 21);
            }

            PlaceStartingUnits();

            _lastPoll = DateTime.Now.AddMinutes(-1);
        }

        public void SetOpponent()
        {
            _opponent = _httpClient.GetOpponent(Id);
        }

        protected override void AddMove(GameMove move)
        {
            move.GameId = Id;
            move.PlayerId = ActivePlayer == _localPlayer ? _us.Id : _opponent.Id;

            base.AddMove(move);

            if (IsMyTurn && !_restoringGame)
            {
                _httpClient.MakeMove(move);
            }
        }

        protected override void EndTurn()
        {
            base.EndTurn();

            SetFogOfWar(_localPlayer); // TODO Make this not needed
        }

        public void SetLocalPlayer(bool blueIsLocal)
        {
            if (blueIsLocal)
            {
                Players.Single(e => e.Color == TileColor.Blue).IsLocalPlayer = true;
            }
            else
            {
                Players.Single(e => e.Color == TileColor.Red).IsLocalPlayer = true;
            }

            SetFogOfWar(_localPlayer);
        }

        protected override void EndGame()
        {
            base.EndGame();

            if (ActivePlayer == _localPlayer)
            {
                // Active player communicates the result
                _httpClient.SetResult(Id, Result);
            }

            if (ActivePlayer == _localPlayer && Result[2] == 'r')
            {
                // No need to notify the player that just resigned that the game ended
                return;
            }

            string message = "The game has ended. ";

            if ((Result[0] == 'b' && _localPlayer.Color == TileColor.Blue)
                || (Result[0] == 'r' && _localPlayer.Color == TileColor.Red))
            {
                message += "You have achieved victory by ";
            }
            else
            {
                message += "You were defeated by ";
            }

            if (Result[2] == 'r')
            {
                message += "resignation.";
            }
            else if (Result[2] == 'c')
            {
                message += "conquest.";
            }
            else
            {
                message += "wonder.";
            }

            Popup = new Popup
            {
                IsInformational = true,
                Message = message
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsEnded)
            {
                return;
            }

            if (IsMyTurn)
            {
                return;
            }

            if ((DateTime.Now - _lastPoll).TotalSeconds < 2)
            {
                return;
            }

            _lastPoll = DateTime.Now;

            PollLastMovesDto dto = _httpClient.GetLatestMoves(Id, MoveHistory.Count);

            if (dto == null)
            {
                return;
            }

            SetFogOfWar(ActivePlayer);

            foreach (GameMove newMove in dto.Moves.OrderBy(e => e.MoveNumber))
            {
                ApplyMove(newMove);
            }

            SetFogOfWar(_localPlayer);

            ClearTemporaryTileColorsExceptPink();
        }

        public void ApplyMoveList(List<GameMove> moveList)
        {
            _restoringGame = true;

            foreach (GameMove move in moveList.OrderBy(e => e.MoveNumber))
            {
                SetFogOfWar(ActivePlayer);
                ApplyMove(move);
            }

            _restoringGame = false;
        }

        private void ApplyMove(GameMove newMove)
        {
            Tile originTile = null;
            Tile destinationTile = null;

            if (newMove.OriginTileId != null)
            {
                originTile = Map.Tiles[newMove.OriginTileId.Value];
            }
            if (newMove.DestinationTileId != null)
            {
                destinationTile = Map.Tiles[newMove.DestinationTileId.Value];
            }

            if (newMove.IsEndOfTurn)
            {
                EndTurn();
                State = GameState.Default;

                if (!_restoringGame)
                {
                    SoundEffectLibrary.YourTurn.Play();
                }
            }
            else if (newMove.IsResign)
            {
                Result = ActivePlayer.Color == TileColor.Blue ? "r+r" : "b+r";
                EndGame();
            }
            else if (newMove.IsMovement)
            {
                ICanMove mover;

                if (newMove.SubselectedUnitHitpoints != null)
                {
                    // Unit is moving out of a group. We know which unit to move out of the group based on the HP parameter.
                    // If there are multiple units with the same HP, it doesn't matter which we move because they are effectively identical
                    mover = ((IContainsUnits)originTile.Object).Units
                        .Where(e => ((PlayerObject)e).HitPoints == newMove.SubselectedUnitHitpoints)
                        .First();
                }
                else
                {
                    mover = (ICanMove)originTile.Object;
                }

                TryMoveObject(originTile, destinationTile, mover);
            }
            else if (newMove.IsAttack)
            {
                TryAttackObject(originTile, destinationTile, (IAttacker)originTile.Object, (IAttackable)destinationTile.Object);
            }
            else if (newMove.IsWaypoint)
            {
                SetWaypoint((ICanMakeUnits)originTile.Object, originTile, destinationTile);
            }
            else if (newMove.IsQueueBuilding)
            {
                Type buildingType = ActivePlayer.Factories
                    .Where(e => e.Type.Name == newMove.BuildingTypeName)
                    .Select(e => e.Type)
                    .Single();

                QueueBuilding(buildingType, (ICanMakeBuildings)originTile.Object, destinationTile);
            }
            else if (newMove.IsQueueUnit)
            {
                Type unitType = ActivePlayer.Factories
                    .Where(e => e.Type.Name == newMove.UnitTypeName)
                    .Select(e => e.Type)
                    .Single();

                TryQueueUnit(unitType, (ICanMakeUnits)originTile.Object);
            }
            else if (newMove.IsQueueResearch)
            {
                Research research = ResearchLibrary.GetByResearchEnum(newMove.ResearchId.Value);
                TryResearch(research, (ICanMakeResearch)originTile.Object);
            }
            else if (newMove.IsCancel)
            {
                Tile tile = Map.Tiles[newMove.OriginTileId.Value];
                CancelQueue((IHasQueue)tile.Object);
            }
            else if (newMove.IsDestroyBuilding)
            {
                Tile tile = Map.Tiles[newMove.OriginTileId.Value];
                DestroyOwnBuilding(tile);
            }
        }
    }
}
