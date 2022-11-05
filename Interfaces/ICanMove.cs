namespace AoeBoardgame
{
    interface ICanMove
    {
        int Speed { get; set; }
        Tile DestinationTile { get; set; }
        int StepsTakenThisTurn { get; set; }
    }
}
