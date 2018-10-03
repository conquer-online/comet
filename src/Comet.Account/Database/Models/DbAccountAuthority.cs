namespace Comet.Account.Database.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Account authority level defines the permissions a player has on the server to
    /// access restricted areas, run game server administrative commands, etc. Linked
    /// from the account table using a foreign key. 
    /// </summary>
    [Table("account_authority")]
    public partial class DbAccountAuthority
    {
        /// <summary>
        /// Initializes navigational properties for <see cref="DbAccountAuthority"/>.
        /// </summary>
        public DbAccountAuthority()
        {
            Account = new HashSet<DbAccount>();
            Realms = new HashSet<DbRealm>();
        }

        // Column Properties
        public ushort AuthorityID { get; set; }
        public string AuthorityName { get; set; }

        // Navigational Properties
        public virtual ICollection<DbAccount> Account { get; set; }
        public virtual ICollection<DbRealm> Realms { get; set; }
    }
}
