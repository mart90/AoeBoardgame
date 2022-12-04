namespace AoeBoardgame.Multiplayer
{
    class MultiplayerGameSettings : GameSettings
    {
        public int MinRating { get; set; }
        public int MaxRating { get; set; }

        public MultiplayerGameSettings()
        {
            MinRating = 1000;
            MaxRating = 2000;
        }
    }
}
