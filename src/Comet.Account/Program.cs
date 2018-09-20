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
            Console.WriteLine();
            Console.WriteLine("         .:'    Comet, Account Server");
            Console.WriteLine("     _.::'      Copyright 2018 \"Spirited\"");
            Console.WriteLine("    (_.'        Version 1.0.0-alpha1");
            Console.WriteLine();

            // Read configuration file and command-line arguments
            Console.WriteLine("Parsing server configuration...");
            Console.WriteLine();
            var config = new ServerConfiguration(args);
            if (!config.Valid) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid server configuration file");
                Console.ResetColor();
                return;
            }
            
            // Output configuration
            Console.WriteLine("   Database Server: {0}", config.Database.Hostname);
            Console.WriteLine("   Database Schema: {0}", config.Database.Schema);
            Console.WriteLine("   Server Listener: {0}:{1}", 
                config.Network.IPAddress, 
                config.Network.Port);

            // Start the server listener
            Console.WriteLine("\nLaunching server listener...");
            var server = new Server(config);
            server.Start(config.Network.Port, config.Network.IPAddress);

            // Output all clear and wait for user input
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Listening for new connections");
            Console.ResetColor();
            Console.WriteLine();
            Console.ReadKey(true);
        }

        
    }
}
