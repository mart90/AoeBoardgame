namespace AoeBoardgame
{
    abstract class GameSettings
    {
        public bool TimeControlEnabled { get; set; }
        public int? StartTimeMinutes { get; set; }
        public int? TimeIncrementSeconds { get; set; }
        public string MapSeed { get; set; }

        public GameSettings()
        {
            TimeControlEnabled = true;
            StartTimeMinutes = 5;
            TimeIncrementSeconds = 5;
        }
    }
}
