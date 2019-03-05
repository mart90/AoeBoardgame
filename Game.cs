using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class Game
    {
        public GameState State { get; set; }
        public List<Player> Players { get; set; }

        private readonly Map _map;

        public Game(List<Player> players, Map map)
        {
            Players = players;
            _map = map;

            players[0].IsActive = true;
            State = GameState.TurnStart;
        }

        public Player ActivePlayer => Players.Single(e => e.IsActive);

        public Tile GetTileByLocation(Point location) => _map.GetTileByLocation(location);

        public void StartTurn()
        {
            ActivePlayer.EmptyQueues();
            State = GameState.MainPhase;
        }

        public void EndMainPhase()
        {
            // TODO end main phase logic

            State = GameState.TurnEnd;
        }

        public void EndTurn()
        {
            // TODO end turn logic

            PassTurnToNextPlayer();
            State = GameState.TurnStart;
        }

        public void SelectTileByLocation(Point location)
        {
            var tile = GetTileByLocation(location);
            if (tile == null)
            {
                return;
            }

            ClearCurrentSelection();
            tile.IsSelected = true;

            if (tile.Object is IHasRange objectWithRange)
            {
                IEnumerable<Tile> tilesInRange = _map.FindTilesInRangeOfSelected(objectWithRange.Range);

                if (tilesInRange != null)
                {
                    foreach (var tileInRange in tilesInRange)
                    {
                        tileInRange.SetTemporaryColor(TileColor.Pink);
                    }
                }
            }
        }

        public void HoverOverTileByLocation(Point location)
        {
            var tile = GetTileByLocation(location);
            if (tile != null)
            {
                ClearCurrentHover();
                tile.IsHovered = true;
                HandleHover();
            }
        }

        public void HandleHover()
        {
            var selectedObject = _map.SelectedTile?.Object;

            if (_map.HoveredTile == _map.SelectedTile || selectedObject == null)
            {
                return;
            }

            if (selectedObject is ICanMove)
            {
                IEnumerable<Tile> pathFromSelectedToHovered = _map.FindPathFromSelectedToHovered();

                if (pathFromSelectedToHovered != null)
                {
                    ClearTemporaryTileColors();
                    foreach (var tile in pathFromSelectedToHovered)
                    {
                        tile.SetTemporaryColor(TileColor.Teal);
                    }
                }
            }
        }

        public void DrawMap(SpriteBatch spriteBatch)
        {
            _map.Draw(spriteBatch);
        }

        public void PlaceStartingTownCenters()
        {
            // For now (maybe ever) assume 2 players
            for (int i = 0; i < 2; i++)
            {
                var player = Players[i];
                var tc = player.AddAndGetPlaceableObject<TownCenter>();

                int tileRow = _map.Height / 2;
                int tileColumn = i == 0 ? _map.Width / 5 : _map.Width - _map.Width / 5 - 1;
                var tile = _map.Tiles[tileRow * _map.Width + tileColumn];

                tile.SetObject(tc);
            }

            _map.Tiles[360].SetObject(Players[0].AddAndGetPlaceableObject<Villager>()); // Test stuff
            _map.Tiles[361].SetObject(Players[1].AddAndGetPlaceableObject<Tower>()); // Test stuff
        }

        public void ClearCurrentSelection()
        {
            var selectedTile = _map.SelectedTile;
            if (selectedTile != null)
            {
                selectedTile.IsSelected = false;
            }

            ClearTemporaryTileColors();
        }

        public void ClearCurrentHover()
        {
            var hoveredTile = _map.HoveredTile;
            if (hoveredTile != null)
            {
                hoveredTile.IsHovered = false;
            }
        }

        private void ClearTemporaryTileColors()
        {
            foreach (var tile in _map.Tiles)
            {
                tile.SetTemporaryColor(TileColor.Default);
            }
        }

        private void PassTurnToNextPlayer()
        {
            var activePlayerId = Players.IndexOf(ActivePlayer);

            ActivePlayer.IsActive = false;

            if (activePlayerId == Players.Count - 1)
            {
                Players[0].IsActive = true;
            }
            else
            {
                Players[activePlayerId + 1].IsActive = true;
            }
        }
    }
}
