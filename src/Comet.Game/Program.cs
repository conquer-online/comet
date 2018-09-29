namespace Comet.Game
{
    using System;

    /// <summary>
    /// The game server listens for authentication players with a valid access token from
    /// the account server, and hosts the game world. The game world in this project has 
    /// been simplified into a single server executable. For an n-server distributed 
    /// systems implementation of a Conquer Online server, see Chimera. 
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Copyright notice may not be commented out. If adding your own copyright or
            // claim of ownership, you may include a second copyright above the existing
            // copyright. Do not remove completely to comply with software license. The
            // project name and version may be removed or changed.
            Console.Title = "Comet, Game Server";
            Console.WriteLine();
            Console.WriteLine("    Comet: Game Server");
            Console.WriteLine("    Copyright 2018 Gareth Jensen \"Spirited\"");
            Console.WriteLine("    All Rights Reserved");
            Console.WriteLine();
        }
    }
}
