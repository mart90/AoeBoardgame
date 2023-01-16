namespace AoeBoardgame
{
    class Knight : PlayerObject,
        IMilitaryUnit,
        ICanMove,
        ICanFormGroup,
        IAttacker,
        IConsumesGold,
        ICanBeUpgraded,
        ICavalry
    {
        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }
        public bool HasAttackedThisTurn { get; set; }

        public Tile DestinationTile { get; set; }
        public int StepsTakenThisTurn { get; set; }

        public int GoldConsumption { get; set; }

        public bool IsSubSelected { get; set; }

        public int UpgradeLevel { get; set; }

        public Knight(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
