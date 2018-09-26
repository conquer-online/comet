namespace Comet.Account.Packets
{
    using System.Text;
    using Comet.Account.States;
    using Comet.Network.Packets;
    using Comet.Network.Security;

    /// <remarks>Packet Type 1051</remarks>
    /// <summary>
    /// Message containing login credentials from the login screen. This is the first
    /// packet sent to the account server from the client on login. The server checks the
    /// encrypted password against the hashed password in the database, the responds with
    /// <see cref="MsgConnectEx"/> with either a pass or fail.
    /// </summary>
    public sealed class MsgAccount : MsgBase<Client>
    {
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
        public override void Process(Client client)
        {

        }

        /// <summary>
        /// Decodes a byte packet into the packet structure defined by this message class. 
        /// Should be invoked to structure data from the client for processing. Decoding
        /// follows TQ Digital's byte ordering rules for an all-binary protocol.
        /// </summary>
        /// <param name="bytes">Bytes from the packet processor or client socket</param>
        public override void Decode(byte[] bytes)
        {
            var rc5 = new RC5();
            var password = new byte[16];

            var reader = new PacketReader(bytes);
            this.Length = reader.ReadUInt16();
            this.Type = (PacketType)reader.ReadUInt16();
            this.Username = reader.ReadString(16);
            rc5.Decrypt(reader.ReadBytes(16), password);
            this.Password = Encoding.ASCII.GetString(password).Trim('\0');
            this.Realm = reader.ReadString(16);
        }
    }
}
