using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    abstract class PlaceableObject
    {
        public Player Owner { get; set; }
        public TileColorTexture ColorTexture { get; set; }

        protected Texture2D Texture;
        protected TextureLibrary TextureLibrary;

        public virtual void Draw(SpriteBatch spriteBatch, Rectangle location)
        {
            spriteBatch.Draw(Texture, location, Color.White);
        }
    }
}
