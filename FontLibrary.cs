using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class FontLibrary
    {
        public SpriteFont DefaultFont { get; }
        public SpriteFont DefaultFontBold { get; }

        public FontLibrary(ContentManager contentManager)
        {
            DefaultFont = contentManager.Load<SpriteFont>("fonts/defaultFont");
            DefaultFontBold = contentManager.Load<SpriteFont>("fonts/defaultFontBold");
        }
    }
}
