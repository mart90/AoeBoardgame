namespace AoeBoardgame
{
    class GameMove
    {
        public int GameId { get; set; }
        public int MoveNumber { get; set; }

        public string PlayerName { get; set; }
        public int? PlayerId { get; set; }

        public int? OriginTileId { get; set; }
        public int? DestinationTileId { get; set; }

        public bool IsMovement { get; set; }
        public bool IsAttack { get; set; }
        public bool IsWaypoint { get; set; }
        public bool IsQueueBuilding { get; set; }
        public bool IsQueueUnit { get; set; }
        public bool IsQueueResearch { get; set; }
        public bool IsCancel { get; set; }
        public bool IsDestroyBuilding { get; set; }

        public int? SubselectedUnitHitpoints { get; set; }
        public string BuildingTypeName { get; set; }
        public string UnitTypeName { get; set; }
        public ResearchEnum? ResearchId { get; set; }

        public bool IsEndOfTurn { get; set; }
        public bool IsResign { get; set; }
    }
}
