namespace AoeBoardgame
{
    static class InterfaceMethods
    {
        public static bool HasSpentAllMovementPoints<T>(this T mover) where T : ICanMove
        {
            return mover.StepsTakenThisTurn == mover.Speed;
        }

        public static bool HasBuildingQueued<T>(this T builder) where T : ICanMakeBuildings
        {
            return builder.BuildingTypeQueued != null;
        }

        public static bool HasSomethingQueued<T>(this T queuer) where T : IHasQueue
        {
            return queuer.QueueTurnsLeft != 0;
        }
    }
}
