namespace Comet.Account.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Account information for a registered player. The account server uses this information
    /// to authenticate the player on login, and track permissions and player access to the
    /// server. Passwords are hashed using a salted SHA-256 for user protection.
    /// </summary>
    [Table("account")]
    public partial class Account
    {
        /// <summary>
        /// Initializes navigational properties for <see cref="Account"/>.
        /// </summary>
        public Account()
        {
            Logins = new HashSet<Logins>();
        }

        // Column Properties
        public uint AccountID { get; set; }
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
        public virtual AccountAuthority Authority { get; set; }
        public virtual AccountStatus Status { get; set; }
        public virtual ICollection<Logins> Logins { get; set; }
    }
}
