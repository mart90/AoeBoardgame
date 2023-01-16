﻿using System.Collections.Generic;

namespace AoeBoardgame
{
    class ThrowingAxeman : PlayerObject,
        IMilitaryUnit,
        ICanMove,
        ICanFormGroup,
        IAttacker,
        IHasRange,
        IConsumesGold,
        ICanBeUpgraded,
        IInfantry
    {
        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }
        public bool HasAttackedThisTurn { get; set; }

        public Tile DestinationTile { get; set; }
        public int StepsTakenThisTurn { get; set; }

        public int GoldConsumption { get; set; }

        public bool IsSubSelected { get; set; }

        public int Range { get; set; }
        public IEnumerable<Tile> RangeableTiles { get; set; }
        public bool HasMinimumRange { get; set; }

        public int UpgradeLevel { get; set; }

        public ThrowingAxeman(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
