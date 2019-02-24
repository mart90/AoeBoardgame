using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{ 
    class Tile
    {
        public TileType Type { get; private set; }
        public bool IsSelected { get; private set; }

        private Texture2D _texture;
        private readonly Rectangle _location;
        private readonly Rectangle _objectLocation;
        private PlaceableObject _object;
        private TileColor _temporaryColor;

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

        public bool LocationSquareIncludesPoint(Point point) =>
            // If true the point is in this tile's square, but could still be in one of the corners
            point.X >= _location.X
            && point.X <= _location.X + _location.Width
            && point.Y >= _location.Y
            && point.Y <= _location.Y + _location.Height;

        public bool IsAccessible() => _object == null && Type == TileType.Dirt;

        public Point Center() => _location.Center;

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
            IsSelected = true;
            if (_object != null)
            {
                _object.IsSelected = true;
            }
        }

        public void RemoveSelection()
        {
            IsSelected = false;
            if (_object != null)
            {
                _object.IsSelected = false;
            }
        }

        public void SetTemporaryColor(TileColor color)
        {
            _temporaryColor = color;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _location, Color.White);

            if (_temporaryColor != TileColor.Default)
            {
                spriteBatch.Draw(_textureLibrary.GetTileColorByType(_temporaryColor), _location, Color.White);
            }

            if (_object != null)
            {
                if (_object.GetType() != typeof(GaiaObject) && _temporaryColor == TileColor.Default)
                {
                    spriteBatch.Draw(
                        _object.IsSelected ? _textureLibrary.GetTileColorByType(TileColor.Green) : _object.ColorTexture.Texture,
                        _location,
                        Color.White);
                }

                _object.Draw(spriteBatch, _objectLocation);
            }
        }
    }
}
