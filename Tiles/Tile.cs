using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{ 
    class Tile
    {
        public TileType Type { get; private set; }

        private Texture2D _texture;
        private readonly Rectangle _location;
        private readonly Rectangle _objectLocation;
        private PlaceableObject _object;

        private readonly TextureLibrary _textureLibrary;
        
        public Tile(TextureLibrary textureLibrary, Rectangle location)
        {
            _textureLibrary = textureLibrary;
            _location = location;

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

        public bool IncludesPoint(Point point) => 
            point.X >= _location.X && point.X <= _location.X + _location.Width
            && point.Y >= _location.Y && point.Y <= _location.Y + _location.Height;

        public bool IsAccessible() => _object == null && Type == TileType.Dirt;

        public void SetType(TileType tileType)
        {
            Type = tileType;
            _texture = _textureLibrary.GetTileTextureByType(tileType);
        }

        public void SetObject(PlaceableObject obj)
        {
            SetType(TileType.Dirt);
            _object = obj;
        }

        public void SetSelected()
        {
            if (_object != null)
            {
                _object.Selected = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _location, Color.White);

            if (_object == null)
            {
                return;
            }

            if (_object.GetType() != typeof(GaiaObject))
            {
                spriteBatch.Draw(
                    _object.Selected ? _textureLibrary.GetTileColorByType(TileColor.Green) : _object.ColorTexture.Texture,
                    _location,
                    Color.White);
            }

            _object.Draw(spriteBatch, _objectLocation);
        }
    }
}
