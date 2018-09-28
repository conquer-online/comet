namespace Comet.Account.Database.Repositories
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Comet.Account.Database.Models;

    /// <summary>
    /// Repository for defining data access layer (DAL) logic for the account table. Allows
    /// the server to fetch account details for player authentication. Accounts are fetched
    /// on demand for each player authentication request.
    /// </summary>
    public static class AccountsRepository
    {
        /// <summary>
        /// Fetches an account record from the database using the player's username as a
        /// unique key for selecting a single record.
        /// </summary>
        /// <param name="username">Username to pull account info for</param>
        /// <returns>Returns account details from the database.</returns>
        public static Account Get(string username)
        {
            using (var db = new ServerDbContext())
                return db.Accounts
                    .Include(x => x.Authority)
                    .Include(x => x.Status)
                    .Where(x => x.Username == username)
                    .SingleOrDefault();
        }
    } 
}