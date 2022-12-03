using System.Collections.Generic;

namespace AoeBoardgame
{
    abstract class PlayerObject : PlaceableObject, IAttackable
    {
        public virtual int HitPoints { get; set; }
        public virtual int MaxHitPoints { get; set; }
        public int MeleeArmor { get; set; }
        public int RangedArmor { get; set; }
        public virtual int LineOfSight { get; set; }
        public Player Owner { get; set; } // TODO circular dependency
        public List<Tile> VisibleTiles { get; set; }

        protected PlayerObject(TextureLibrary textureLibrary, Player owner)
        {
            TextureLibrary = textureLibrary;
            Texture = TextureLibrary.GetObjectTextureByType(GetType());
            Owner = owner;
            VisibleTiles = new List<Tile>();

            ColorTexture = new TileColorTexture
            {
                Texture = textureLibrary.GetTileColorByType(Owner.Color),
                TileColor = Owner.Color
            };
        }
    }
}
