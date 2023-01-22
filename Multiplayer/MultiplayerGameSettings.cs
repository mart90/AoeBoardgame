namespace AoeBoardgame
{
    class MultiplayerGameSettings
    {
        public int MinRating { get; set; }
        public int MaxRating { get; set; }

        public int? RestoreGameId { get; set; }
        public int? RestoreMoveNumber { get; set; }

        public bool HostPlaysBlue { get; set; }

        public bool IsTimeControlEnabled { get; set; }
        public int? StartTimeMinutes { get; set; }
        public int? TimeIncrementSeconds { get; set; }

        public string MapSeed { get; set; }

        public MultiplayerGameSettings()
        {
            MinRating = 1000;
            MaxRating = 2000;
        }
    }
}
