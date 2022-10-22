namespace Comet.Account.Database.Tests
{
    using System;
    using Xunit;
    using Comet.Account.Database;

    /// <summary>
    /// Validates that the server can connect to a MySQL database using the EF Core MySQL
    /// extension and connector. Validates context configuration and model building. 
    /// </summary>
    public class ContextTests
    {
        [Theory]
        [InlineData("--Database:Hostname=mysql")]
        public void ContextConfiguration(params string[] args)
        {
            var config = new ServerConfiguration(args);
            ServerDbContext.Configuration = config.Database;
            try 
            {
                using var db = new ServerDbContext();
            }
            catch (Exception ex) { Assert.Null(ex); }
        }
    }
}
