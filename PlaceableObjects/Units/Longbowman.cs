using System.Collections.Generic;

namespace AoeBoardgame
{
    class Longbowman : PlayerObject,
        IMilitaryUnit,
        ICanMove,
        ICanFormGroup,
        IAttacker,
        IHasRange,
        IConsumesFood,
        ICanBeUpgraded,
        IArcher
    {
        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }
        public bool HasAttackedThisTurn { get; set; }

        public Tile DestinationTile { get; set; }
        public int StepsTakenThisTurn { get; set; }

        public int FoodConsumption { get; set; }

        public bool IsSubSelected { get; set; }

        public int Range { get; set; }
        public IEnumerable<Tile> RangeableTiles { get; set; }
        public bool HasMinimumRange { get; set; }

        public int UpgradeLevel { get; set; }

        public Longbowman(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
