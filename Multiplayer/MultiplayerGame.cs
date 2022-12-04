using AoeBoardgame.Multiplayer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class MultiplayerGame : Game, IUiWindow
    {
        public int Id { get; set; }

        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }

        private readonly User _us;
        private User _opponent;

        private readonly MultiplayerHttpClient _httpClient;

        private DateTime _lastPoll;

        private bool _myTurn;

        public string MapSeed => Map.Seed;

        private Player _localPlayer => Players.Single(e => e.IsLocalPlayer);

        protected override Player VisiblePlayer => _localPlayer;

        public MultiplayerGame(
            MultiplayerGameSettings settings,
            TextureLibrary textureLibrary, 
            FontLibrary fontLibrary, 
            ResearchLibrary researchLibrary,
            MultiplayerHttpClient httpClient) : base(textureLibrary, fontLibrary, researchLibrary)
        {
            CorrespondingUiState = UiState.MultiplayerGame;
            _httpClient = httpClient;

            _us = _httpClient.AuthenticatedUser;

            Players = new List<Player>()
            {
                new Player("Blue", new England(textureLibrary, researchLibrary), TileColor.Blue)
                {
                    IsActive = true
                },
                new Player("Red", new France(textureLibrary, researchLibrary), TileColor.Red)
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

            StartTurn();
        }

        public void SetOpponent()
        {
            _opponent = _httpClient.GetOpponent(Id);
        }

        protected override void MakeMove(GameMove move)
        {
            move.GameId = Id;
            move.PlayerId = State == GameState.OpponentTurn ? _opponent.Id : _us.Id;

            base.MakeMove(move);

            if (_myTurn)
            {
                _httpClient.MakeMove(move);

                if (move.IsEndOfTurn)
                {
                    State = GameState.OpponentTurn;
                    _myTurn = false;
                }
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
                _myTurn = true;
            }
            else
            {
                Players.Single(e => e.Color == TileColor.Red).IsLocalPlayer = true;
                State = GameState.OpponentTurn;
                _myTurn = false;
            }

            SetFogOfWar(_localPlayer);
        }

        public override void Update(SpriteBatch spriteBatch)
        {
            base.Update(spriteBatch);

            //if (GameEnded)
            //{
            //    return;
            //}

            if (_myTurn)
            {
                return;
            }

            if ((DateTime.Now - _lastPoll).TotalSeconds < 2)
            {
                return;
            }

            _lastPoll = DateTime.Now;

            PollLastMovesDto dto = _httpClient.LatestMoves(Id, MoveHistory.Count);

            if (dto == null)
            {
                return;
            }

            SetFogOfWar(ActivePlayer);

            foreach (var newMove in dto.Moves.OrderBy(e => e.MoveNumber))
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
                    _myTurn = true;
                }
                else if (newMove.IsMovement)
                {
                    TryMoveObject(originTile, destinationTile, (ICanMove)originTile.Object);
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
            }

            SetFogOfWar(_localPlayer);
        }
    }
}
