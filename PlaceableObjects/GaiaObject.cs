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
