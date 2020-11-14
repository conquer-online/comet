namespace Comet.Account.Packets
{
    using System.Text;
    using System.Threading.Tasks;
    using Comet.Account.Database.Repositories;
    using Comet.Account.States;
    using Comet.Network.Packets;
    using Comet.Network.Security;
    using Comet.Shared.Models;
    using static Comet.Account.Packets.MsgConnectEx;

    /// <remarks>Packet Type 1051</remarks>
    /// <summary>
    /// Message containing login credentials from the login screen. This is the first
    /// packet sent to the account server from the client on login. The server checks the
    /// encrypted password against the hashed password in the database, the responds with
    /// <see cref="MsgConnectEx"/> with either a pass or fail.
    /// </summary>
    public sealed class MsgAccount : MsgBase<Client>
    {
        // Packet Properties
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Realm { get; private set; }

        /// <summary>
        /// Process can be invoked by a packet after decode has been called to structure
        /// packet fields and properties. For the server implementations, this is called
        /// in the packet handler after the message has been dequeued from the server's
        /// <see cref="PacketProcessor"/>.
        /// </summary>
        /// <param name="client">Client requesting packet processing</param>
        public override async Task ProcessAsync(Client client)
        {
            // Fetch account info from the database
            client.Account = await AccountsRepository.FindAsync(this.Username).ConfigureAwait(false);
            if (client.Account == null || !AccountsRepository.CheckPassword(
                this.Password, client.Account.Password, client.Account.Salt))
            {
                await client.SendAsync(new MsgConnectEx(RejectionCode.InvalidPassword));
                client.Socket.Disconnect(false);
                return;
            }

            // Connect to the game server
            Database.Models.DbRealm server;
            if (!Kernel.Realms.TryGetValue(this.Realm, out server) || !server.Rpc.Online)
            {
                await client.SendAsync(new MsgConnectEx(RejectionCode.ServerDown));
                client.Socket.Disconnect(false);
                return;
            }

            // Get an access token from the server
            var args = new TransferAuthArgs();
            args.AccountID = client.Account.AccountID;
            args.AuthorityID = client.Account.AuthorityID;
            args.AuthorityName = client.Account.Authority.AuthorityName;
            args.IPAddress = client.IPAddress;

            ulong token = await server.Rpc.CallAsync<ulong>("TransferAuth", args);
            await client.SendAsync(new MsgConnectEx(server.GameIPAddress, server.GamePort, token));
        }

        /// <summary>
        /// Decodes a byte packet into the packet structure defined by this message class. 
        /// Should be invoked to structure data from the client for processing. Decoding
        /// follows TQ Digital's byte ordering rules for an all-binary protocol.
        /// </summary>
        /// <param name="bytes">Bytes from the packet processor or client socket</param>
        public override void Decode(byte[] bytes)
        {
            var reader = new PacketReader(bytes);
            this.Length = reader.ReadUInt16();
            this.Type = (PacketType)reader.ReadUInt16();
            this.Username = reader.ReadString(16);
            this.Password = this.DecryptPassword(reader.ReadBytes(16));
            this.Realm = reader.ReadString(16);
        }

        /// <summary>
        /// Decrypts the password from read in packet bytes for the <see cref="Decode"/>
        /// method. Trims the end of the password string of null terminators.
        /// </summary>
        /// <param name="buffer">Bytes from the packet buffer</param>
        /// <returns>Returns the decrypted password string.</returns>
        private string DecryptPassword(byte[] buffer)
        {
            var rc5 = new RC5();
            var password = new byte[16];
            rc5.Decrypt(buffer, password);
            return Encoding.ASCII.GetString(password).Trim('\0');
        }
    }
}
