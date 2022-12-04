using System;

namespace AoeBoardgame.Multiplayer
{
    class Lobby
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int HostId { get; set; }
        public DateTime HostLastPing { get; set; }
        public string Status { get; set; }

        public bool IsSelected { get; set; }

        public MultiplayerGameSettings Settings { get; set; }

        public string Title()
        {
            string title = Settings.MapSeed != null ? "Seeded - " : "Generated - ";

            if (Settings.TimeControlEnabled)
            {
                title += $"{Settings.StartTimeMinutes}+{Settings.TimeIncrementSeconds} - ";
            }
            else
            {
                title += "Unlimited time - ";
            }

            title += $"Rating {Settings.MinRating}-{Settings.MaxRating}";

            return title;
        }
    }
}
