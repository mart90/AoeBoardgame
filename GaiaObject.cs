using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeBoardgame
{
    class GaiaObject : PlaceableObject
    {
        public GaiaObject(TextureLibrary textureLibrary, PlaceableObjectType objectType)
        {
            TextureLibrary = textureLibrary;
            SetType(objectType);
        }
    }
}
