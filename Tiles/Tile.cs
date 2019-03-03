using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{ 
    class Tile
    {
        public TileType Type { get; private set; }
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }
        public PlaceableObject Object { get; private set; }

        private Texture2D _texture;
        private readonly Rectangle _location;
        private readonly Rectangle _objectLocation;
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

        // If true the point is in this tile's square, but could still be in one of the corners which would overlap with another tile
        public bool LocationSquareIncludesPoint(Point point) => _location.Contains(point);

        public bool IsAccessible() => Object == null && Type == TileType.Dirt;

        public Point Center() => _location.Center;

        public void SetType(TileType tileType)
        {
            Type = tileType;
            _texture = _textureLibrary.GetTileTextureByType(tileType);
        }

        public void SetObject(PlaceableObject obj)
        {
            SetType(TileType.Dirt);
            Object = obj;
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

            if (Object != null)
            {
                if (Object.GetType().BaseType != typeof(GaiaObject) && _temporaryColor == TileColor.Default)
                {
                    spriteBatch.Draw(
                        IsSelected ? _textureLibrary.GetTileColorByType(TileColor.Green) : Object.ColorTexture.Texture,
                        _location,
                        Color.White);
                }

                Object.Draw(spriteBatch, _objectLocation);
            }
        }
    }
}
