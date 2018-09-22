namespace Comet.Account.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("logins")]
    public partial class Logins
    {
        public DateTime Timestamp { get; set; }
        public uint AccountID { get; set; }
        public string IPAddress { get; set; }

        public virtual Account Account { get; set; }
    }
}
