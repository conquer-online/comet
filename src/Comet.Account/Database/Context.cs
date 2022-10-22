namespace Comet.Account.Database
{
    using Microsoft.EntityFrameworkCore;
    using Comet.Account.Database.Models;

    /// <summary>
    /// Server database client context implemented using Entity Framework Core, an open
    /// source object-relational mapping framework for ADO.NET. Substitutes in MySQL 
    /// support through a third-party framework provided by Pomelo Foundation.
    /// </summary>
    public partial class ServerDbContext : DbContext
    {
        // Connection String Configuration
        public static ServerConfiguration.DatabaseConfiguration Configuration;

        // Table Definitions
        public virtual DbSet<DbAccount> Accounts { get; set; }
        public virtual DbSet<DbAccountAuthority> AccountAuthorities { get; set; }
        public virtual DbSet<DbAccountStatus> AccountStatuses { get; set; }
        public virtual DbSet<DbLogin> Logins { get; set; }
        public virtual DbSet<DbRealm> Realms { get; set; }

        /// <summary>
        /// Configures the database to be used for this context. This method is called
        /// for each instance of the context that is created. For this project, the MySQL
        /// connector will be initialized with a connection string from the server's
        /// configuration file.
        /// </summary>
        /// <param name="options">Builder to create the context</param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLazyLoadingProxies(false);
            options.UseMySql(string.Format("server={0};database={1};user={2};password={3}",
                ServerDbContext.Configuration.Hostname, 
                ServerDbContext.Configuration.Schema,
                ServerDbContext.Configuration.Username, 
                ServerDbContext.Configuration.Password),
                MySqlServerVersion.LatestSupportedServerVersion);
        }

        /// <summary>
        /// Typically called only once when the first instance of the context is created.
        /// Allows for model building before the context is fully initialized, and used
        /// to initialize composite keys and relationships.
        /// </summary>
        /// <param name="builder">Builder for creating models in the context</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DbAccount>(e => e.HasKey(x => x.AccountID));
            builder.Entity<DbAccountAuthority>(e => e.HasKey(x => x.AuthorityID));
            builder.Entity<DbAccountStatus>(e => e.HasKey(x => x.StatusID));
            builder.Entity<DbLogin>(e => e.HasKey(x => new { x.AccountID, x.Timestamp }));
            builder.Entity<DbRealm>(e => e.HasKey(x => x.RealmID));
        }

        /// <summary>
        /// Tests that the database connection is alive and configured.
        /// </summary>
        public static bool Ping()
        {
            try 
            {
                using ServerDbContext ctx = new ServerDbContext();
                return ctx.Database.CanConnect();
            }
            catch { return false; }
        }
    }
}
