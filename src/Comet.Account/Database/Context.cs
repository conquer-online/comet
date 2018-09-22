namespace Comet.Account.Database
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Server database client context implemented using Entity Framework Core, an open
    /// source object-relational mapping framework for ADO.NET. Substitutes in MySQL 
    /// support through a third-party framework provided by Oracle.
    /// </summary>
    public sealed class ServerDbContext : DbContext
    {
        // Connection String Configuration
        public static ServerConfiguration.DatabaseConfiguration Configuration;

        /// <summary>
        /// Configures the database to be used for this context. This method is called
        /// for each instance of the context that is created. For this project, the MySQL
        /// connector will be initialized with a connection string from the server's
        /// configuration file.
        /// </summary>
        /// <param name="optionsBuilder">Builder to create the context</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(string.Format(
                "server={0};database={1};user={2};password={3}",
                Configuration.Hostname, Configuration.Schema,
                Configuration.Username, Configuration.Password));
        }
    }
}
