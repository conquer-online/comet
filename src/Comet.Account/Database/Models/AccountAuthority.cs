namespace Comet.Account.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("account_authority")]
    public partial class AccountAuthority
    {
        public AccountAuthority()
        {
            Account = new HashSet<Account>();
        }

        [Key]
        public ushort AuthorityID { get; set; }
        public string AuthorityName { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}
