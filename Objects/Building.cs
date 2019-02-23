namespace AoeBoardgame
{
    class Building : PlaceableObject
    {
        public Building(TextureLibrary textureLibrary, PlaceableObjectType buildingType, Player owner)
        {
            TextureLibrary = textureLibrary;

            SetType(buildingType);
            Owner = owner;

            ColorTexture = new TileColorTexture
            {
                Texture = textureLibrary.GetTileColorByType(Owner.Color),
                TileColor = Owner.Color
            };
        }
    }
}
