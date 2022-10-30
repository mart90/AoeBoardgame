namespace AoeBoardgame.Extensions
{
    static class InterfaceMethods
    {
        public static bool HasSpentAllMovementPoints<T>(this T mover) where T : ICanMove
        {
            return mover.StepsTakenThisTurn == mover.Speed;
        }
    }
}
