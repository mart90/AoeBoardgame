using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class TextureLibrary
    {
        public Texture2D Grass { get; set; }
        public Texture2D Forest { get; set; }
        public Texture2D StoneMine { get; set; }

        public TextureLibrary(ContentManager contentManager)
        {
            Grass = contentManager.Load<Texture2D>("Tiles/Terrain/Grass/grass_05");
            Forest = contentManager.Load<Texture2D>("Tiles/Terrain/Grass/grass_12");
            StoneMine = contentManager.Load<Texture2D>("Tiles/Terrain/Dirt/dirt_18");
        }
    }
}
