﻿namespace AoeBoardgame
{
    class Scout : PlayerObject,
        IMilitaryUnit,
        ICanMove,
        ICanFormGroup,
        IAttacker,
        IConsumesFood,
        ICavalry
    {
        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }
        public bool HasAttackedThisTurn { get; set; }

        public Tile DestinationTile { get; set; }
        public int StepsTakenThisTurn { get; set; }

        public int FoodConsumption { get; set; }

        public bool IsSubSelected { get; set; }

        public Scout(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
