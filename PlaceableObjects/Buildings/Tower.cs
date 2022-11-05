using System;

namespace AoeBoardgame
{
    class Tower : PlayerObject,
        IAttacker,
        IHasRange
    {
        public int AttackDamage { get; set; }
        public int Range { get; set; }

        public Tower(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
