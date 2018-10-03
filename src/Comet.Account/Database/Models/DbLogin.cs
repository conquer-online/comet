namespace Comet.Account.Database.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Record of a successful login to the account server. These records can be used to
    /// debug client connectivity issues, or outline connection patterns used to identify
    /// wrongful logins.
    /// </summary>
    [Table("logins")]
    public partial class DbLogin
    {
        // Column Properties
        public DateTime Timestamp { get; set; }
        public uint AccountID { get; set; }
        public string IPAddress { get; set; }

        // Navigational Properties
        public virtual DbAccount Account { get; set; }
    }
}
