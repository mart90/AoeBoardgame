namespace AoeBoardgame
{
    class Challenge
    {
        public int Id { get; set; }
        public ChallengeType ChallengeType { get; set; }
        public string UiName { get; set; }
        public string Description { get; set; }
        public string MapSeed { get; set; }
        public int BronzeTurns { get; set; }
        public int SilverTurns { get; set; }
        public int GoldTurns { get; set; }
    }
}
