namespace AoeBoardgame
{
    interface ICanMove
    {
        int Speed { get; set; }
        Tile DestinationTile { get; set; }
        int StepsTakenThisTurn { get; set; }
    }

    static class ICanMoveMethods
    {
        public static bool HasSpentAllMovementPoints<T>(this T mover) where T : ICanMove
        {
            if (mover.StepsTakenThisTurn > mover.Speed)
            {
                throw new System.Exception("Mover somehow took more steps than allowed this turn");
            }

            return mover.StepsTakenThisTurn == mover.Speed;
        }
    }
}
