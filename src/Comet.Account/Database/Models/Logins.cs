namespace Comet.Account.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Records of successful logins to the account server. These records can be used to
    /// debug client connectivity issues, or outline connection patterns used to identify
    /// wrongful logins.
    /// </summary>
    [Table("logins")]
    public partial class Logins
    {
        // Column Properties
        public DateTime Timestamp { get; set; }
        public uint AccountID { get; set; }
        public string IPAddress { get; set; }

        // Navigational Properties
        public virtual Account Account { get; set; }
    }
}
