namespace AoeBoardgame
{
    class Unit : PlaceableObject, IAttackable
    {
        public int HitPoints { get; set; }

        public Unit(TextureLibrary textureLibrary, PlaceableObjectType unitType, Player owner)
        {
            TextureLibrary = textureLibrary;

            SetType(unitType);
            Owner = owner;

            ColorTexture = new TileColorTexture
            {
                Texture = textureLibrary.GetTileColorByType(Owner.Color),
                TileColor = Owner.Color
            };
        }
    }
}
