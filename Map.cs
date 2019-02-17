using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class Map
    {
        public List<List<Tile>> TileRows { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tileRow in TileRows)
            {
                foreach (var tile in tileRow)
                {
                    spriteBatch.Draw(tile.Texture, tile.Location, Color.White);
                }
            }
        }

        public int TileCount => TileRows.Count * TileRows[0].Count;

        public Tile GetTileById(int id)
        {
            int rowSize = TileRows[0].Count;

            return TileRows[id / rowSize][id % rowSize];
        }
    }
}
