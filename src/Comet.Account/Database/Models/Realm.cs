namespace Comet.Account.Database.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Realms are configured instances of the game server. This class defines routing 
    /// details for authenticated clients to be redirected to. Redirection involves
    /// access token leasing, provided by the game server via RPC. Security for RPC stream
    /// encryption is defined in this class.
    /// </summary>
    [Table("realm")]
    public partial class Realm
    {
        // Column Properties
        public uint RealmID { get; set; }
        public string Name { get; set; }
        public ushort AuthorityID { get; set; }
        public string IPAddress { get; set; }
        public uint Port { get; set; }
        public string CipherAlgorithm { get; set; }
        public string CipherKey { get; set; }
        public string CipherIV { get; set; }

        // Navigational Properties
        public virtual AccountAuthority Authority { get; set; }
    }
}