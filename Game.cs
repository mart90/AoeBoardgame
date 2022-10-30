using AoeBoardgame.Extensions;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

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
            State = GameState.MyTurn;
        }

        public Player ActivePlayer => Players.Single(e => e.IsActive);

        public Tile GetTileByLocation(Point location) => _map.GetTileByLocation(location);

        private PlaceableObject SelectedObject => _map.SelectedTile?.Object;

        public void StartTurn()
        {
            SetFogOfWar();
        }

        public void EndTurn()
        {
            MoverTakeSteps();

            // TODO queues, resources

            PassTurnToNextPlayer();
        }

        public void SetFogOfWar()
        {
            _map.ResetFogOfWar();

            foreach (PlayerObject obj in ActivePlayer.OwnedObjects)
            {
                List<Tile> tiles = new PathFinder(_map).GetAllTilesInRange(_map.GetTileByObject(obj), obj.LineOfSight).ToList();
                tiles.ForEach(e => e.HasFogOfWar = false);
            }
        }

        public void MoverTakeSteps()
        {
            foreach (ICanMove mover in ActivePlayer.OwnedObjects.Where(e => e is ICanMove).Cast<ICanMove>())
            {
                if (mover.DestinationTile != null && !mover.HasSpentAllMovementPoints())
                {
                    Tile sourceTile = _map.GetTileByObject((PlayerObject)mover);
                    var path = _map.FindPath(sourceTile, mover.DestinationTile);

                    if (path == null)
                    {
                        // TODO highlight unit, message "the highlighted unit's destination has become invalid. Its path is reset"
                        mover.DestinationTile = null;
                        continue;
                    }

                    _map.ProgressOnPath(mover, path);

                    if (_map.GetTileByObject((PlayerObject)mover) == mover.DestinationTile)
                    {
                        mover.DestinationTile = null;
                    }
                }

                mover.StepsTakenThisTurn = 0;
            }
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
                    tilesInRange.Highlight(TileColor.Pink);
                }
            }

            if (!ActivePlayer.OwnedObjects.Contains(tile.Object))
            {
                tile.IsSelected = false;
                return;
            }

            if (tile.Object is ICanMove mover)
            {
                if (mover.DestinationTile != null)
                {
                    var path = _map.FindPath(_map.GetTileByObject(tile.Object), mover.DestinationTile);
                    path.Highlight(TileColor.Orange);
                }
            }
            if (tile.Object is ICanMakeBuildings builder)
            {
                foreach (var buildingType in builder.BuildingTypesAllowedToMake)
                {

                }
            }
        }

        public void SetDestinationByLocation(Point location)
        {
            if (!(SelectedObject is ICanMove mover))
            {
                return;
            }

            var sourceTile = _map.GetTileByObject(SelectedObject);
            var destinationTile = GetTileByLocation(location);

            if (destinationTile == sourceTile)
            {
                mover.DestinationTile = null;
                return;
            }

            var path = _map.FindPath(sourceTile, destinationTile);

            if (path == null)
            {
                return;
            }

            mover.DestinationTile = destinationTile;

            path.Highlight(TileColor.Orange);

            if (!mover.HasSpentAllMovementPoints())
            {
                _map.ProgressOnPath(mover, path);
                SetFogOfWar();

                if (_map.GetTileByObject((PlayerObject)mover) == mover.DestinationTile)
                {
                    mover.DestinationTile = null;
                }
            }
        }

        public void HoverOverTileByLocation(Point location)
        {
            ClearCurrentHover();

            var tile = GetTileByLocation(location);
            if (tile != null)
            {
                tile.IsHovered = true;
                HandleHover();
            }
        }

        public void HandleHover()
        {
            var selectedObject = _map.SelectedTile?.Object;

            if (selectedObject == null)
            {
                return;
            }

            if (selectedObject is ICanMove mover)
            {
                ClearTemporaryTileColors();
                if (mover.DestinationTile != null)
                {
                    var path = _map.FindPath(_map.GetTileByObject(selectedObject), mover.DestinationTile);
                    path.Highlight(TileColor.Orange);
                }

                if (_map.HoveredTile == _map.SelectedTile)
                {
                    return;
                }

                IEnumerable <Tile> pathFromSelectedToHovered = _map.FindPathFromSelectedToHovered();

                if (pathFromSelectedToHovered != null)
                {
                    pathFromSelectedToHovered.Highlight(TileColor.Teal);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _map.Draw(spriteBatch);

            ImGui.SetWindowSize(new System.Numerics.Vector2(500, 1060));
            ImGui.SetWindowPos(new System.Numerics.Vector2(1480, -20));

            if (ImGui.Button("End turn", new System.Numerics.Vector2(100, 40)))
            {
                EndTurn();
            }
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

            _map.Tiles[360].SetObject(Players[0].AddAndGetPlaceableObject<Villager>());
            _map.Tiles[120].SetObject(Players[1].AddAndGetPlaceableObject<Villager>());

            var builder = (ICanMakeBuildings) Players[0].OwnedObjects.Find(e => e is ICanMakeBuildings);

            if (_map.Tiles[361].IsAccessible)
            {
                Players[0].MakeBuilding<Tower>(builder, _map.Tiles[361]);
            }
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

            if (SelectedObject is ICanMove)
            {
                ClearTemporaryTileColors();
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

            StartTurn();
        }
    }
}
