using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class Game
    {
        public List<Player> Players { get; set; }

        private readonly Map _map;
        private readonly TextureLibrary _textureLibrary;

        public Game(TextureLibrary textureLibrary, List<Player> players, Map map)
        {
            _textureLibrary = textureLibrary;
            Players = players;
            _map = map;
        }

        public Player ActivePlayer => Players.Find(e => e.IsActive);

        public Tile GetTileByLocation(Point location) => _map.GetTileByLocation(location);

        public void SelectTileByLocation(Point location)
        {
            var tile = GetTileByLocation(location);
            if (tile != null)
            {
                ClearCurrentSelection();
                tile.IsSelected = true;
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

            if (selectedObject.GetType().IsSubclassOf(typeof(Unit)))
            {
                IEnumerable<Tile> pathFromSelectedToHovered =
                    FindPath(_map.SelectedTile, _map.HoveredTile);

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
                var building = new Building(_textureLibrary, PlaceableObjectType.TownCenter, player);

                int tileRow = _map.Height / 2;
                int tileColumn = i == 0 ? _map.Width / 5 : _map.Width - _map.Width / 5 - 1;
                var tile = _map.Tiles[tileRow * _map.Width + tileColumn];

                tile.SetObject(building);
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

            ClearTemporaryTileColors();
        }

        private void ClearTemporaryTileColors()
        {
            foreach (var tile in _map.Tiles)
            {
                tile.SetTemporaryColor(TileColor.Default);
            }
        }

        public IEnumerable<Tile> FindPath(Tile origin, Tile destination)
        {
            return new PathFinder(_map)
                .GetOptimalPath(_map.Tiles.IndexOf(origin), _map.Tiles.IndexOf(destination));
        }
    }
}
