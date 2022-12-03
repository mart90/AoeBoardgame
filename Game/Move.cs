namespace AoeBoardgame
{
    class Move
    {
        public string PlayerName { get; set; }

        public int OriginTileId { get; set; }
        public int DestinationTileId { get; set; }

        public bool IsMovement { get; set; }
        public bool IsAttack { get; set; }
        public bool IsWayPoint { get; set; }
        public bool IsQueueBuilding { get; set; }
        public bool IsQueueUnit { get; set; }
        public bool IsQueueResearch { get; set; }

        public string BuildingTypeName { get; set; }
        public string UnitTypeName { get; set; }
        public ResearchEnum? ResearchEnum { get; set; }

        public bool IsEndOfTurn { get; set; }
    }
}
