using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class TileObject
    {
        public TileObjectType Type { get; private set; }

        private Texture2D _texture;

        private readonly TextureLibrary _textureLibrary;

        public TileObject(TextureLibrary textureLibrary)
        {
            _textureLibrary = textureLibrary;
        }

        public void SetType(TileObjectType tileObjectType)
        {
            Type = tileObjectType;
            _texture = _textureLibrary.GetTileObjectTextureByType(tileObjectType);
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle location)
        {
            spriteBatch.Draw(_texture, location, Color.White);
        }
    }
}
