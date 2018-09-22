namespace Comet.Account.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("account")]
    public partial class Account
    {
        public Account()
        {
            Logins = new HashSet<Logins>();
        }

        public uint AccountID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ushort AuthorityID { get; set; }
        public ushort StatusID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string IPAddress { get; set; }
        public DateTime Registered { get; set; }

        public virtual AccountAuthority Authority { get; set; }
        public virtual AccountStatus Status { get; set; }
        public virtual ICollection<Logins> Logins { get; set; }
    }
}
