namespace Comet.Game.Packets
{
    using System;
    using Comet.Game.States;
    using Comet.Network.Packets;

    /// <remarks>Packet Type 1009</remarks>
    /// <summary>
    /// Message containing an item action command. Item actions are usually performed to 
    /// manage player equipment, inventory, money, or item shop purchases and sales. It
    /// is serves a second purpose for measuring client ping.
    /// </summary>
    public sealed class MsgItem : MsgBase<Client>
    {
        // Packet Properties
        public uint CharacterID { get; set; }
        public uint Command { get; set; }
        public uint Action { get; set; }
        public uint Timestamp { get; set; }
        public uint Argument { get; set; }

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
            this.Action = reader.ReadUInt32();
            this.Timestamp = reader.ReadUInt32();
            this.Argument = reader.ReadUInt32();
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
            writer.Write((uint)this.Action);
            writer.Write(this.Timestamp);
            writer.Write(this.Argument);
            return writer.ToArray();
        }

        /// <summary>
        /// Process can be invoked by a packet after decode has been called to structure
        /// packet fields and properties. For the server implementations, this is called
        /// in the packet handler after the message has been dequeued from the server's
        /// <see cref="PacketProcessor"/>.
        /// </summary>
        /// <param name="client">Client requesting packet processing</param>
        public override void Process(Client client)
        {
            switch (this.Action)
            {
                default:
                    client.Send(this);
                    Console.WriteLine(
                        "Missing packet {0}, Length {1}\n{2}", 
                        this.Type, this.Length, PacketDump.Hex(this.Encode()));
                    break;
            }
        }

        /// <summary>
        /// Enumeration type for defining item actions that may be requested by the user, 
        /// or given to by the server. Allows for action handling as a packet subtype.
        /// </summary>
        public enum ItemActionType
        {
            Ping = 27
        }
    }
}