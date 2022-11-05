using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{ 
    class Tile
    {
        /// <summary>
        /// Index in the tile array in Map
        /// </summary>
        public int Id { get; set; }

        public TileType Type { get; private set; }
        public bool IsSelected { get; set; }
        public bool IsViewed { get; set; } // Selected but no actions possible (opponent's tile)
        public bool IsHovered { get; set; }
        public bool HasFogOfWar { get; set; }
        public PlaceableObject Object { get; private set; }
        public TileColor TemporaryColor { get; set; }
        public bool BuildingUnderConstruction { get; set; }

        private Texture2D _texture;
        private readonly Rectangle _location;
        private readonly Rectangle _objectLocation;
        private readonly Rectangle _hourglassLocation;
        private readonly Rectangle _hitpointsLocation;

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

            _hourglassLocation = new Rectangle(
                _objectLocation.X + (_objectLocation.Width / 2),
                _objectLocation.Y + (_objectLocation.Height / 10),
                _objectLocation.Width - (_objectLocation.Width / 8),
                _objectLocation.Height - (_objectLocation.Height / 8));

            _hitpointsLocation = new Rectangle(
                _location.X,
                _objectLocation.Y,
                (_location.Width - _objectLocation.Width) / 2,
                (_location.Height - _objectLocation.Height) / 2);
        }

        public bool LocationSquareIncludesPoint(Point point) => _location.Contains(point);

        public bool SeemsAccessible => (Object == null || HasFogOfWar) && Type == TileType.Dirt;

        public Point Center => _location.Center;

        public void SetType(TileType tileType)
        {
            Type = tileType;
            _texture = _textureLibrary.GetTileTextureByType(tileType);
        }

        public void SetObject(PlaceableObject obj)
        {
            if (obj is GaiaObject)
            {
                Type = TileType.Dirt;
            }

            Object = obj;
        }

        public void SetTemporaryColor(TileColor color)
        {
            TemporaryColor = color;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _location, Color.White);

            if (TemporaryColor != TileColor.Default)
            {
                spriteBatch.Draw(_textureLibrary.GetTileColorByType(TemporaryColor), _location, Color.White);
            }

            if (HasFogOfWar)
            {
                spriteBatch.Draw(_textureLibrary.FogOfWar, _location, Color.White);
            }
            else
            {
                if (BuildingUnderConstruction)
                {
                    spriteBatch.Draw(_textureLibrary.TileUnderConstruction, _objectLocation, Color.White);
                }
                else if (Object != null)
                {
                    if (Object.GetType().BaseType != typeof(GaiaObject) && TemporaryColor == TileColor.Default)
                    {
                        spriteBatch.Draw(
                            IsSelected ? _textureLibrary.GetTileColorByType(TileColor.Green) : Object.ColorTexture.Texture,
                            _location,
                            Color.White);
                    }

                    Object.Draw(spriteBatch, _objectLocation);

                    if (Object is IHasQueue queuer && queuer.HasSomethingQueued())
                    {
                        spriteBatch.Draw(_textureLibrary.SomethingInQueue, _hourglassLocation, Color.White);
                    }

                    if (Object is IAttackable attackableObject)
                    {
                        // TODO HP bar
                    }
                }
            }
        }
    }
}
