using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeBoardgame
{
    class Building : PlaceableObject
    {
        public Building(TextureLibrary textureLibrary, PlaceableObjectType buildingType, TileColor color)
        {
            TextureLibrary = textureLibrary;

            SetType(buildingType);

            ColorTexture = new TileColorTexture
            {
                Texture = textureLibrary.GetTileColorByType(color),
                TileColor = color
            };
        }
    }
}
