﻿using System;

namespace AoeBoardgame
{
    class LobbyDto
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int HostId { get; set; }
        public DateTime HostLastPing { get; set; }
        public string Status { get; set; }
        public bool TimeControlEnabled { get; set; }
        public int? StartTimeMinutes { get; set; }
        public int? TimeIncrementSeconds { get; set; }
        public string MapSeed { get; set; }
        public int MinRating { get; set; }
        public int MaxRating { get; set; }
        public int? RestoreGameId { get; set; }
        public int? RestoreMoveNumber { get; set; }
        public bool HostIsBlue { get; set; }

        public Lobby ToLobby()
        {
            return new Lobby
            {
                Id = Id,
                CreatedAt = CreatedAt,
                HostId = HostId,
                HostLastPing = HostLastPing,
                Status = Status,
                Settings = new MultiplayerGameSettings
                {
                    IsTimeControlEnabled = TimeControlEnabled,
                    StartTimeMinutes = StartTimeMinutes,
                    TimeIncrementSeconds = TimeIncrementSeconds,
                    MapSeed = MapSeed,
                    MinRating = MinRating,
                    MaxRating = MaxRating,
                    RestoreGameId = RestoreGameId,
                    RestoreMoveNumber = RestoreMoveNumber,
                    HostPlaysBlue = HostIsBlue
                }
            };
        }
    }
}
