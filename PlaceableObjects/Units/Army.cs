namespace AoeBoardgame
{
    class Army : PlayerObject,
        ICanMove,
        IAttacker,
        IAttackable
    {
        public Army(TextureLibrary textureLibrary, Player owner) : base(textureLibrary, owner)
        {
        }

        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public Tile DestinationTile { get; set; }
        public int StepsTakenThisTurn { get; set; }
    }
}
