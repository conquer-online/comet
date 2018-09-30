namespace Comet.Account.Database
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Defines the configuration file structure for the Account Server. App Configuration 
    /// files are copied to the build output directory on successful build, containing all
    /// default configuration settings for the server, only if the file is newer than the
    /// file bring replaced.
    /// </summary>
    public class ServerConfiguration 
    {
        // Properties and fields
        public DatabaseConfiguration Database { get; set; }
        public NetworkConfiguration Network { get; set; }

        /// <summary>
        /// Encapsulates network configuration for the server listener.
        /// </summary>
        public class NetworkConfiguration
        {
            public string IPAddress { get; set; }
            public int Port { get; set; }
            public int MaxConn { get; set; }
        }

        /// <summary>
        /// Encapsulates database configuration for Entity Framework.
        /// </summary>
        public class DatabaseConfiguration
        {
            public string Hostname { get; set; }
            public string Schema { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// Encapsulates authentication settings for configuring JSON-RPC security between
        /// the account server and all connecting game servers. Key and IV should be the
        /// same across all game server instances.
        /// </summary>
        public class AuthenticationConfiguration
        {
            public string Key { get; set; }
            public string IV { get; set; }
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="ServerConfiguration"/> with command-line
        /// arguments from the user and a configuration file for the application. Builds the
        /// configuration file and binds to this instance of the ServerConfiguration class.
        /// </summary>
        /// <param name="args">Command-line arguments from the user</param>
        public ServerConfiguration(string[] args)
        {
            new ConfigurationBuilder()
                .AddJsonFile("Comet.Account.config")
                .AddCommandLine(args)
                .Build()
                .Bind(this);
        }

        /// <summary>
        /// Returns true if the server configuration is valid after reading.
        /// </summary>
        public bool Valid => this.Database != null && this.Network != null;
    }
}