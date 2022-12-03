namespace AoeBoardgame
{
    class Pikeman : PlayerObject,
        IMilitaryUnit,
        ICanMove,
        ICanFormGroup,
        IAttacker,
        IConsumesFood,
        ICanBeUpgraded,
        IInfantry
    {
        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }
        public bool HasAttackedThisTurn { get; set; }

        public Tile DestinationTile { get; set; }
        public int StepsTakenThisTurn { get; set; }

        public int FoodConsumption { get; set; }

        public bool IsSubSelected { get; set; }

        public int UpgradeLevel { get; set; }

        public Pikeman(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
