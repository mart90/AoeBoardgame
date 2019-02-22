using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeBoardgame
{
    class Unit : PlaceableObject
    {
        public Unit(TextureLibrary textureLibrary, PlaceableObjectType unitType, TileColor color)
        {
            TextureLibrary = textureLibrary;

            SetType(unitType);

            ColorTexture = new TileColorTexture
            {
                Texture = textureLibrary.GetTileColorByType(color),
                TileColor = color
            };
        }
    }
}
