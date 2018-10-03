namespace Comet.Account.Database.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Account status identifier for account standings, which define how the server permits
    /// access to an account. For example, an account may be locked in cases where the
    /// player's password was entered but secondary authentication failed. An account may
    /// also be banned from the server, and not permitted access.
    /// </summary>
    [Table("account_status")]
    public partial class DbAccountStatus
    {
        /// <summary>
        /// Initializes navigational properties for <see cref="DbAccountAuthority"/>.
        /// </summary>
        public DbAccountStatus()
        {
            Account = new HashSet<DbAccount>();
        }

        // Column Properties
        public ushort StatusID { get; set; }
        public string StatusName { get; set; }

        // Navigational Properties
        public virtual ICollection<DbAccount> Account { get; set; }
    }
}
