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
    public partial class AccountAuthority
    {
        /// <summary>
        /// Initializes navigational properties for <see cref="AccountAuthority"/>.
        /// </summary>
        public AccountAuthority()
        {
            Account = new HashSet<Account>();
        }

        // Column Properties
        public ushort AuthorityID { get; set; }
        public string AuthorityName { get; set; }

        // Navigational Properties
        public virtual ICollection<Account> Account { get; set; }
    }
}
