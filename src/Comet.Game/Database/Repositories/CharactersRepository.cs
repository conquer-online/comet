namespace Comet.Game.Database.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Comet.Game.Database.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Repository for defining data access layer (DAL) logic for the character table. Allows
    /// the server to fetch character information for player login. Characters are fetched
    /// on demand, and character names are indexed for fast lookup during character creation.
    /// </summary>
    public static class CharactersRepository
    {
        /// <summary>
        /// Fetches a character record from the database using the character's name as a
        /// unique key for selecting a single record. Character name is indexed for fast
        /// lookup when logging in. 
        /// </summary>
        /// <param name="name">Character's name</param>
        /// <returns>Returns character details from the database.</returns>
        public static async Task<DbCharacter> FindAsync(string name)
        {
            using (var db = new ServerDbContext())
                return await db.Characters
                    .Where(x => x.Name == name)
                    .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Fetches a character record from the database using the character's associated
        /// AccountID as a unique key for selecting a single record.
        /// </summary>
        /// <param name="accountID">Primary key for fetching character info</param>
        /// <returns>Returns character details from the database.</returns>
        public static async Task<DbCharacter> FindAsync(uint accountID)
        {
            using (var db = new ServerDbContext())
                return await db.Characters
                    .Where(x => x.AccountID == accountID)
                    .SingleOrDefaultAsync();
        }

        /// <summary>Checks if a character exists in the database by name.</summary>
        /// <param name="name">Character's name</param>
        /// <returns>Returns true if the character exists.</returns>
        public static async Task<bool> ExistsAsync(string name)
        {
            using (var db = new ServerDbContext())
                return await db.Characters
                    .Where(x => x.Name == name)
                    .AnyAsync();
        }

        /// <summary>
        /// Creates a new character using a character model. If the character primary key
        /// already exists, then character creation will fail. 
        /// </summary>
        /// <param name="character">Character model to be inserted to the database</param>
        public static async Task CreateAsync(DbCharacter character)
        {
            using (var db = new ServerDbContext())
            {
                db.Characters.Add(character);
                await db.SaveChangesAsync();
            }
        }
    } 
}
