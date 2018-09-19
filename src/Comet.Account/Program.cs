namespace Comet.Account
{
    using System;

    /// <summary>
    /// The account server accepts clients and authenticates players from the client's 
    /// login screen. If the player enters valid account credentials, then the server
    /// will send login details to the game server and disconnect the client. The client
    /// will reconnect to the game server with an access token from the account server.
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Copyright notice may not be commented out. If adding your own copyright or
            // claim of ownership, you may include a second copyright above the existing
            // copyright. Do not remove completely to comply with software license. The
            // project name and version may be removed or changed.
            Console.Title = "Comet, Account Server";
            Console.WriteLine("Comet, Account Server");
            Console.WriteLine("Copyright 2018 Gareth Jensen, \"Spirited\"");
            Console.WriteLine("Version 1.0.0-alpha1");
            Console.WriteLine();

            // Read configuration file and command-line arguments
            var config = new ServerConfiguration(args);

            // Start the server listener
            Console.WriteLine("Launching server...");
            var server = new Server(config);
            server.Start(config.Network.Port, config.Network.IPAddress);
            Console.WriteLine("Listening for new connections");
            Console.ReadKey(true);
        }
    }
}
