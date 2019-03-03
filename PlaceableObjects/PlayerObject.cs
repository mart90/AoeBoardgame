namespace AoeBoardgame
{
    abstract class PlayerObject : PlaceableObject, IAttackable
    {
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }

        protected PlayerObject(TextureLibrary textureLibrary, Player owner)
        {
            TextureLibrary = textureLibrary;
            Texture = TextureLibrary.GetObjectTextureByType(GetType());
            Owner = owner;

            ColorTexture = new TileColorTexture
            {
                Texture = textureLibrary.GetTileColorByType(Owner.Color),
                TileColor = Owner.Color
            };
        }

        protected void AddHitPoints(int hitPoints)
        {
            HitPoints += hitPoints;
            MaxHitPoints += hitPoints;
        }

        public abstract void UpgradeToFeudalAge();
        public abstract void UpgradeToCastleAge();
        public abstract void UpgradeToImperialAge();
    }
}
