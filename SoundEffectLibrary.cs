using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace AoeBoardgame
{
    class SoundEffectLibrary
    {
        public SoundEffect YourTurn { get; private set; }

        public SoundEffectLibrary(ContentManager contentManager)
        {
            YourTurn = contentManager.Load<SoundEffect>("SoundEffects/your_turn");
        }
    }
}
