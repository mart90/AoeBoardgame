using ImGuiNET;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui;

namespace AoeBoardgame
{
    class FontLibrary
    {
        public SpriteFont DefaultFont { get; }
        public SpriteFont DefaultFontBold { get; }
        public ImFontPtr TitleFont { get; }

        public FontLibrary(ContentManager contentManager)
        {
            DefaultFont = contentManager.Load<SpriteFont>("fonts/defaultFont");
            DefaultFontBold = contentManager.Load<SpriteFont>("fonts/defaultFontBold");

            // create the object on the native side
            TitleFont = ImGui.GetIO().Fonts.AddFontFromFileTTF("Content/Fonts/Hamlet-Tertia18.otf", 64);
        }
    }
}
