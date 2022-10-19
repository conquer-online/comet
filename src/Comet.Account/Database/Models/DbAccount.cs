namespace Comet.Account.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Account information for a registered player. The account server uses this information
    /// to authenticate the player on login, and track permissions and player access to the
    /// server. Passwords are hashed using a salted SHA-1 for user protection.
    /// </summary>
    [Table("account")]
    public partial class DbAccount
    {
        /// <summary>
        /// Initializes navigational properties for <see cref="DbAccount"/>.
        /// </summary>
        public DbAccount()
        {
            Logins = new HashSet<DbLogin>();
        }

        // Column Properties
        public uint AccountID { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public ushort AuthorityID { get; set; }
        public ushort StatusID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string IPAddress { get; set; }
        public DateTime Registered { get; set; }

        // Navigational Properties
        public virtual DbAccountAuthority Authority { get; set; }
        public virtual DbAccountStatus Status { get; set; }
        public virtual ICollection<DbLogin> Logins { get; set; }
    }
}
