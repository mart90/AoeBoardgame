using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    abstract class PlaceableObject
    {
        public PlaceableObjectType Type { get; private set; }
        public Player Owner { get; set; }
        public bool Selected { get; set; }
        public TileColorTexture ColorTexture { get; set; }

        private Texture2D _texture;
        protected TextureLibrary TextureLibrary;

        public void SetType(PlaceableObjectType objectType)
        {
            Type = objectType;
            _texture = TextureLibrary.GetObjectTextureByType(objectType);
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle location)
        {
            spriteBatch.Draw(_texture, location, Color.White);
        }
    }
}
