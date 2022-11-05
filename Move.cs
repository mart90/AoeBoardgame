namespace AoeBoardgame
{
    class Move
    {
        public string PlayerName { get; set; }

        public int SourceTileId { get; set; }
        public int DestinationTileId { get; set; }

        public bool IsMovement { get; set; }

        public bool IsObjectPlacement { get; set; }
        public string PlacedObjectTypeName { get; set; }

        public bool IsResearch { get; set; }
        public ResearchEnum? ResearchEnum { get; set; }

        public bool IsEndOfTurn { get; set; }
    }
}
