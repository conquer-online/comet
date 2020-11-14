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
        public uint CharacterID { get; set; }
        public uint Command { get; set; }
        public ushort[] Arguments { get; set; }
        public uint Timestamp { get; set; }
        public ActionType Action { get; set; }
        public ushort Direction { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public uint Map { get; set; }
        public uint Color { get; set; }

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
            this.CharacterID = reader.ReadUInt32();
            this.Command = reader.ReadUInt32();
            this.Arguments = new ushort[2];
            for (int i = 0; i < this.Arguments.Length; i++)
                this.Arguments[i] = reader.ReadUInt16();
            this.Timestamp = reader.ReadUInt32();
            this.Action = (ActionType)reader.ReadUInt16();
            this.Direction = reader.ReadUInt16();
            this.X = reader.ReadUInt16();
            this.Y = reader.ReadUInt16();
            this.Map = reader.ReadUInt32();
            this.Color = reader.ReadUInt32();
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
            writer.Write(this.CharacterID);
            writer.Write(this.Command);
            for (int i = 0; i < this.Arguments.Length; i++)
                writer.Write(this.Arguments[i]);
            writer.Write(this.Timestamp);
            writer.Write((ushort)this.Action);
            writer.Write(this.Direction);
            writer.Write(this.X);
            writer.Write(this.Y);
            writer.Write(this.Map);
            writer.Write(this.Color);
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
                    this.Command = client.Character.MapID;
                    this.X = client.Character.X;
                    this.Y = client.Character.Y;
                    await client.SendAsync(this);
                    break;

                case ActionType.LoginComplete:
                    await client.SendAsync(this);
                    break;

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
            LoginSpawn = 74,
            LoginInventory,
            LoginRelationships,
            LoginProficiencies,
            LoginSpells,
            CharacterDirection,
            CharacterEmote = 81,
            MapPortal = 85,
            MapTeleport,
            CharacterLevelUp = 92,
            SpellAbortXp,
            CharacterRevive,
            CharacterDelete,
            CharacterPkMode,
            LoginGuild,
            MapMine = 99,
            MapTeamLeaderStar = 101,
            MapQuery,
            MapSkyColor = 104,
            MapTeamMemberStar = 106,
            MapKickBack = 108,
            SpellRemove,
            ProficiencyRemove,
            BoothSpawn,
            BoothSuspend,
            BoothResume,
            BoothLeave,
            ClientCommand = 116,
            CharacterObservation,
            SpellAbortTransform,
            SpellAbortFlight = 120,
            MapGold,
            RelationshipsEnemy = 123,
            ClientDialog = 126,
            LoginComplete = 132,
            MapEffect,
            LoginOfflineMessages,
            MapRemoveSpawn = 135,
            MapJump = 137,
            CharacterDead = 145,
            MapTeleportEnd = 146,
            RelationshipsFriend = 148,
            CharacterAvatar = 151,
            CharacterPartnerInfo,
            CharacterAway = 161,
            MapPathfinding
        }
    }
}