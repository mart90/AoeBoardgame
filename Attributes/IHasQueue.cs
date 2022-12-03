namespace AoeBoardgame
{
    interface IHasQueue
    {
        int QueueTurnsLeft { get; set; }
    }

    static class IHasQueueMethods
    {
        public static bool HasSomethingQueued<T>(this T queuer) where T : IHasQueue
        {
            return queuer.QueueTurnsLeft != 0;
        }
    }
}
