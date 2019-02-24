using System.Collections.Generic;

namespace AoeBoardgame
{
    class Game
    {
        public List<Player> Players { get; set; }
        public Map Map { get; set; }

        private readonly TextureLibrary _textureLibrary;

        public Game(TextureLibrary textureLibrary, List<Player> players, Map map)
        {
            _textureLibrary = textureLibrary;
            Players = players;
            Map = map;
        }

        public void PlaceStartingTownCenters()
        {
            // For now (maybe ever) assume 2 players
            for (int i = 0; i < 2; i++)
            {
                var player = Players[i];
                var building = new Building(_textureLibrary, PlaceableObjectType.TownCenter, player);

                int tileRow = Map.Height / 2;
                int tileColumn = i == 0 ? Map.Width / 5 : Map.Width - Map.Width / 5 - 1;
                var tile = Map.Tiles[tileRow * Map.Width + tileColumn];

                tile.SetObject(building);
            }
        }

        public void ClearTemporaryTileColors()
        {
            foreach (var tile in Map.Tiles)
            {
                tile.SetTemporaryColor(TileColor.Default);
            }
        }

        public List<Tile> FindPathFromTileToTile(Tile origin, Tile destination)
        {
            return new PathFinder(Map).GetOptimalPath(Map.Tiles.IndexOf(origin), Map.Tiles.IndexOf(destination));
        }
    }
}
