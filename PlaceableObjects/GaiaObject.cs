namespace AoeBoardgame
{
    class GaiaObject : PlaceableObject
    {
        public GaiaObject(TextureLibrary textureLibrary)
        {
            TextureLibrary = textureLibrary;
            Texture = TextureLibrary.GetObjectTextureByType(GetType());
        }
    }
}
