namespace Comet.Game.Packets
{
    using System;
    using System.Threading.Tasks;
    using Comet.Game.Database.Repositories;
    using Comet.Game.States;
    using Comet.Network.Packets;
    using Comet.Shared.Models;
    using static Comet.Game.Packets.MsgTalk;

    /// <remarks>Packet Type 1052</remarks>
    /// <summary>
    /// Message containing a connection request to the game server. Contains the player's
    /// access token from the Account server, and the patch and language versions of the
    /// game client.
    /// </summary>
    public sealed class MsgConnect : MsgBase<Client>
    {
        // Static properties from server initialization
        public static bool StrictAuthentication { get; set; }

        // Packet Properties
        public ulong Token { get; set; }
        public ushort Patch { get; set; }
        public string Language { get; set; }
        public int Version { get; set; }

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
            this.Token = reader.ReadUInt64();
            this.Patch = reader.ReadUInt16();
            this.Language = reader.ReadString(10);
            this.Version = Convert.ToInt32(reader.ReadInt32().ToString(), 2);
        }

        /// <summary>
        /// Process can be invoked by a packet after decode has been called to structure
        /// packet fields and properties. For the server implementations, this is called
        /// in the packet handler after the message has been dequeued from the server's
        /// <see cref="PacketProcessor"/>.
        /// </summary>
        /// <param name="client">Client requesting packet processing</param>
        public override async Task ProcessAsync(Client client)
        {
            // Validate access token
            var auth = Kernel.Logins.Get(this.Token.ToString()) as TransferAuthArgs;
            if (auth == null || (StrictAuthentication && auth.IPAddress == client.IPAddress))
            {
                await client.SendAsync(new MsgTalk(0, TalkChannel.Login, 
                    "Authentication transfer failed. Please try again."));
                client.Socket.Disconnect(false);
                return;
            }

            // Generate new keys and check for an existing character
            client.Cipher.GenerateKeys(new object[] { this.Token });
            var character = CharactersRepository.Get(auth.AccountID);
            if (character == null)
            {
                // Create a new character
                client.Creation = new Creation();
                client.Creation.AccountID = auth.AccountID;
                client.Creation.Token = (uint)this.Token;
                Kernel.Registration.Add(client.Creation.Token);
                await client.SendAsync(new MsgTalk(0, TalkChannel.Login, MsgTalk.NEWROLE));
            }
            else
            {
                // Character already exists
                client.Character = new Character(character);
                await client.SendAsync(new MsgTalk(0, TalkChannel.Login, MsgTalk.ANSWEROK));
                await client.SendAsync(new MsgUserInfo(client.Character));
            }            
        }
    }
}