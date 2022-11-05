using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    abstract class PlayerObject : PlaceableObject, IAttackable
    {
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public int Armor { get; set; }
        public int LineOfSight { get; set; }

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
    }
}
