using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace AoeBoardgame
{
    class SoundEffectLibrary
    {
        public SoundEffect YourTurn { get; private set; }
        public SoundEffect ButtonClick { get; private set; }
        public Song ThemeSong { get; private set; }

        public SoundEffectLibrary(ContentManager contentManager)
        {
            YourTurn = contentManager.Load<SoundEffect>("SoundEffects/your_turn");
            ButtonClick = contentManager.Load<SoundEffect>("SoundEffects/button_click");
            ThemeSong = contentManager.Load<Song>("Music/ThemeSong");
        }
    }
}
