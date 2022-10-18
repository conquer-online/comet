namespace Comet.Game
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Comet.Game.Database;
    using Comet.Game.Packets;
    using Comet.Network.RPC;
    using Comet.Network.Security;

    /// <summary>
    /// The game server listens for authentication players with a valid access token from
    /// the account server, and hosts the game world. The game world in this project has 
    /// been simplified into a single server executable. For an n-server distributed 
    /// systems implementation of a Conquer Online server, see Chimera. 
    /// </summary>
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            // Copyright notice may not be commented out. If adding your own copyright or
            // claim of ownership, you may include a second copyright above the existing
            // copyright. Do not remove completely to comply with software license. The
            // project name and version may be removed or changed.
            Console.Title = "Comet, Game Server";
            Console.WriteLine();
            Console.WriteLine("  Comet: Game Server");
            Console.WriteLine("  Spirited (C) All Rights Reserved");
            Console.WriteLine("  Patch 5065");
            Console.WriteLine();

            // Read configuration file and command-line arguments
            var config = new ServerConfiguration(args);
            if (!config.Valid) 
            {
                Console.WriteLine("Invalid server configuration file");
                return;
            }

            // Initialize the database
            Console.WriteLine("Initializing server...");
            MsgConnect.StrictAuthentication = config.Authentication.StrictAuthPass;
            ServerDbContext.Configuration = config.Database;
            if (!ServerDbContext.Ping())
            {
                Console.WriteLine("Invalid database configuration");
                return;
            }

            // Start background services
            var tasks = new List<Task>();
            tasks.Add(Kernel.Services.Randomness.StartAsync(CancellationToken.None));
            tasks.Add(DiffieHellman.ProbablePrimes.StartAsync(CancellationToken.None));
            await Task.WhenAll(tasks.ToArray());
            
            // Start the RPC server listener
            Console.WriteLine("Launching server listeners...");
            var rpcServer = new RpcServerListener(new Remote());
            var rpcServerTask = rpcServer.StartAsync(config.RpcNetwork.Port, config.RpcNetwork.IPAddress);

            // Start the game server listener
            var server = new Server(config);
            var serverTask = server.StartAsync(config.GameNetwork.Port, config.GameNetwork.IPAddress);

            // Output all clear and wait for user input
            Console.WriteLine("Listening for new connections");
            Console.WriteLine();
            
            await rpcServerTask;
            await serverTask;
        }
    }
}
