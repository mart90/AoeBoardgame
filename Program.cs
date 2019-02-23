using System;

namespace AoeBoardgame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new GameFrontEnd())
                game.Run();
        }
    }
#endif
}
