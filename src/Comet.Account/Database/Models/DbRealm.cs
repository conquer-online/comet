namespace Comet.Account.Database.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Comet.Network.RPC;

    /// <summary>
    /// Realms are configured instances of the game server. This class defines routing 
    /// details for authenticated clients to be redirected to. Redirection involves
    /// access token leasing, provided by the game server via RPC. Security for RPC stream
    /// encryption is defined in this class.
    /// </summary>
    [Table("realm")]
    public partial class DbRealm
    {
        // Column Properties
        public uint RealmID { get; private set; }
        public string Name { get; set; }
        public ushort AuthorityID { get; set; }
        public string GameIPAddress { get; set; }
        public uint GamePort { get; set; }
        public string RpcIPAddress { get; set; }
        public uint RpcPort { get; set; }

        // Navigational Properties
        public virtual DbAccountAuthority Authority { get; set; }

        // Application Logic Fields
        [NotMapped]
        public RpcClient Rpc;
    }
}