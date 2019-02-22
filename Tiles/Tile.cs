using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class Tile
    {
        public TileType Type { get; private set; }
        public bool Selected { get; set; }

        private Texture2D _texture;
        private readonly Rectangle _location;
        private readonly Rectangle _objectLocation;
        private readonly TileColorTexture _colorTexture;
        private TileObject _object;

        private readonly TextureLibrary _textureLibrary;
        
        public Tile(TextureLibrary textureLibrary, Rectangle location)
        {
            _textureLibrary = textureLibrary;
            _location = location;

            _colorTexture = new TileColorTexture();

            // Largest possible square inside hexagon has dimensions 0.634 * a, where a is the hexagon height & width
            // Give any object to be placed on this tile these (max) dimensions
            int objectDimensions = (int)Math.Floor(_location.Width * 0.634);
            int fromEdgePixels = (_location.Width - objectDimensions) / 2;

            _objectLocation = new Rectangle(
                _location.X + fromEdgePixels,
                _location.Y + fromEdgePixels,
                objectDimensions,
                objectDimensions);
        }

        public bool IncludesPoint(Point point)
        {
            return point.X >= _location.X && point.X <= _location.X + _location.Width
                && point.Y >= _location.Y && point.Y <= _location.Y + _location.Height;
        }

        public void SetType(TileType tileType)
        {
            Type = tileType;
            _texture = _textureLibrary.GetTileTextureByType(tileType);
        }

        public void SetObject(TileObjectType tileObjectType)
        {
            _object = new TileObject(_textureLibrary);
            _object.SetType(tileObjectType);
        }

        public void SetColor(TileColor tileColor)
        {
            _colorTexture.TileColor = tileColor;
            _colorTexture.Texture = _textureLibrary.GetTileColorByType(tileColor);
        }

        public void RemoveObject()
        {
            _object = null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _location, Color.White);

            if (Selected)
            {
                spriteBatch.Draw(_textureLibrary.GetTileColorByType(TileColor.Green), _location, Color.White);
            }
            else if (_colorTexture.TileColor != TileColor.Default)
            {
                spriteBatch.Draw(_colorTexture.Texture, _location, Color.White);
            }

            _object?.Draw(spriteBatch, _objectLocation);
        }
    }
}
