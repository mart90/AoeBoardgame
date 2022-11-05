using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AoeBoardgame
{
    class FontLibrary
    {
        public SpriteFont DefaultFontBold { get; }

        public FontLibrary(ContentManager contentManager)
        {
            DefaultFontBold = contentManager.Load<SpriteFont>("fonts/defaultFontBold");
        }
    }
}
