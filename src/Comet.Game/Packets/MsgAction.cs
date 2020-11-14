namespace Comet.Game.Packets
{
    using System;
    using System.Threading.Tasks;
    using Comet.Game.States;
    using Comet.Network.Packets;

    /// <remarks>Packet Type 1010</remarks>
    /// <summary>
    /// Message containing a general action being performed by the client. Commonly used
    /// as a request-response protocol for question and answer like exchanges. For example,
    /// walk requests are responded to with an answer as to if the step is legal or not.
    /// </summary>
    public sealed class MsgAction : MsgBase<Client>
    {
        // Packet Properties
        public uint Timestamp { get; set; }
        public uint CharacterID { get; set; }
        public uint Command { get; set; }
        public ushort[] Arguments { get; set; }
        public uint Direction { get; set; }
        public ActionType Action { get; set; }

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
            this.Timestamp = reader.ReadUInt32();
            this.CharacterID = reader.ReadUInt32();
            this.Arguments = new ushort[2];
            for (int i = 0; i < this.Arguments.Length; i++)
                this.Arguments[i] = reader.ReadUInt16();
            this.Command = reader.ReadUInt32();
            this.Direction = reader.ReadUInt32();
            this.Action = (ActionType)reader.ReadUInt32();
        }

        /// <summary>
        /// Encodes the packet structure defined by this message class into a byte packet
        /// that can be sent to the client. Invoked automatically by the client's send 
        /// method. Encodes using byte ordering rules interoperable with the game client.
        /// </summary>
        /// <returns>Returns a byte packet of the encoded packet.</returns>
        public override byte[] Encode()
        {
            var writer = new PacketWriter();
            writer.Write((ushort)base.Type);
            writer.Write(this.Timestamp);
            writer.Write(this.CharacterID);
            for (int i = 0; i < this.Arguments.Length; i++)
                writer.Write(this.Arguments[i]);
            writer.Write(this.Command);
            writer.Write(this.Direction);
            writer.Write((uint)this.Action);
            return writer.ToArray();
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
            switch (this.Action)
            {
                case ActionType.LoginSpawn:
                    this.CharacterID = client.ID;
                    this.Direction = client.Character.MapID;
                    this.Arguments[0] = client.Character.X;
                    this.Arguments[1] = client.Character.Y;
                    await client.SendAsync(this);
                    break;

                //case ActionType.LoginComplete:
                //    await client.SendAsync(this);
                //    break;

                default:
                    await client.SendAsync(this);
                    await client.SendAsync(new MsgTalk(client.ID, MsgTalk.TalkChannel.Service,
                        String.Format("Missing packet {0}, Action {1}, Length {2}",
                        this.Type, this.Action, this.Length)));
                    Console.WriteLine(
                        "Missing packet {0}, Action {1}, Length {2}\n{3}", 
                        this.Type, this.Action, this.Length, PacketDump.Hex(this.Encode()));
                    break;
            }
        }

        /// <summary>
        /// Defines actions that may be requested by the user, or given to by the server.
        /// Allows for action handling as a packet subtype. Enums should be named by the 
        /// action they provide to a system in the context of the player actor.
        /// </summary>
        public enum ActionType
        {
            CharacterDirection = 124,
            CharacterEmote = 126,
            MapPortal = 130,
            MapTeleport = 131,
            LoginSpawn = 137,
            LoginInventory,
            LoginRelationships,
            MapRemoveSpawn = 141,
            MapJump,
            SpellRemove = 144,
            ProficiencyRemove,
            CharacterLevelUp,
            SpellAbortXp,
            CharacterRevive,
            CharacterDelete,
            LoginProficiencies,
            LoginSpells,
            CharacterPkMode,
            LoginGuild,
            CharacterDead,
            RelationshipsFriend = 156,
            MapMine = 159,
            MapEffect = 162,
            MapKickBack = 164,
            BoothSpawn = 167,
            BoothSuspend,
            BoothResume,
            BoothLeave,
            ClientCommand = 172,
            SpellAbortTransform = 174,
            CharacterObservation,
            SpellAbortFlight,
            MapGold,
            ClientDialog = 186,
            LoginComplete = 190,
            MapQuery = 232
        }
    }
}