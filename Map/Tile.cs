using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AoeBoardgame
{
    class Tile
    {
        /// <summary>
        /// Index in the tile array in Map
        /// </summary>
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public TileType Type { get; private set; }
        public bool HasStartingTownCenter { get; set; }
        public bool IsSelected { get; set; }
        public bool IsViewed { get; set; } // Selected but no actions possible (opponent's tile)
        public bool IsHovered { get; set; }
        public bool HasFogOfWar { get; set; }
        public PlaceableObject Object { get; private set; }
        public TileColor TemporaryColor { get; set; }
        public bool BuildingUnderConstruction { get; set; }
        public bool IsScouted { get; set; }

        private Texture2D _texture;
        private readonly Rectangle _location;
        private readonly Rectangle _objectLocation;
        private readonly Rectangle _hourglassLocation;
        private readonly Rectangle _leftSideLocation;
        private readonly Rectangle _rightsideLocation;

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

            _rightsideLocation = new Rectangle(
                _objectLocation.X + _objectLocation.Width - (_objectLocation.Width / 6),
                _objectLocation.Y - _objectLocation.Height / 3,
                _objectLocation.Width / 3,
                _objectLocation.Height);

            _leftSideLocation = new Rectangle(
                _location.X,
                _objectLocation.Y,
                (_location.Width - _objectLocation.Width) / 2,
                (_location.Height - _objectLocation.Height) / 2);
        }

        public bool LocationSquareIncludesPoint(Point point) => _location.Contains(point);

        public bool SeemsAccessible => (Object == null || (HasFogOfWar && !IsScouted)) && Type == TileType.Dirt;

        public bool SeemsAccessibleGaia => Object == null && Type == TileType.Dirt;

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
                SetType(TileType.Dirt);
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
                if (Object != null && !(Object is ICanMove) && IsScouted)
                {
                    Object.Draw(spriteBatch, _objectLocation);

                    if (!(Object is GaiaObject))
                    {
                        spriteBatch.Draw(Object.ColorTexture.Texture, _location, Color.White);
                    }
                }

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
                    if (!(Object is GaiaObject) && TemporaryColor == TileColor.Default)
                    {
                        spriteBatch.Draw(
                            IsSelected || IsViewed ? _textureLibrary.GetTileColorByType(TileColor.Green) : Object.ColorTexture.Texture,
                            _location,
                            Color.White);
                    }

                    Object.Draw(spriteBatch, _objectLocation);

                    if (Object is IHasQueue queuer && queuer.HasSomethingQueued())
                    {
                        spriteBatch.Draw(_textureLibrary.SomethingInQueue, _hourglassLocation, Color.White);
                    }

                    if (Object is IContainsUnits army)
                    {
                        for (int i = 0; i < army.Units.Count; i++)
                        {
                            spriteBatch.Draw(_textureLibrary.ArmyStar, new Rectangle(
                                _rightsideLocation.X,
                                _rightsideLocation.Y + _objectLocation.Height - (_objectLocation.Height / 4) * i,
                                _rightsideLocation.Width,
                                _rightsideLocation.Height / 4), Color.White);
                        }
                    }

                    if (Object is PlayerObject playerObject && playerObject.HitPoints < playerObject.MaxHitPoints)
                    {
                        int hpBars = (int)Math.Ceiling((double)playerObject.HitPoints / playerObject.MaxHitPoints * 4);

                        for (int i = 0; i < hpBars; i++)
                        {
                            spriteBatch.Draw(_textureLibrary.HpBar, new Rectangle(
                                _leftSideLocation.X + 3,
                                _leftSideLocation.Y - 8 + _objectLocation.Height - (_objectLocation.Height / 4 - 1) * i,
                                _leftSideLocation.Width - 2,
                                _leftSideLocation.Height / 2), Color.White);
                        }
                    }
                }
            }
        }
    }
}
