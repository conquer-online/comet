namespace Comet.Account.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("account_status")]
    public partial class AccountStatus
    {
        public AccountStatus()
        {
            Account = new HashSet<Account>();
        }

        [Key]
        public ushort StatusID { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}
