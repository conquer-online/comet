namespace Comet.Game.Packets
{
    using System;
    using Comet.Game.Database.Models;
    using Comet.Game.Database.Repositories;
    using Comet.Game.States;
    using Comet.Network.Packets;
    using static Comet.Game.Packets.MsgTalk;

    /// <remarks>Packet Type 1001</remarks>
    /// <summary>
    /// Message containing character creation details, such as the new character's name,
    /// body size, and profession. The character name should be verified, and may be
    /// rejected by the server if a character by that name already exists.
    /// </summary>
    public sealed class MsgRegister : MsgBase<Client>
    {
        // Packet Properties
        public string Username { get; set; }
        public string CharacterName { get; set; }
        public string MaskedPassword { get; set; }
        public ushort Body { get; set; }
        public ushort Class { get; set; }
        public uint Token { get; set; }

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
            this.CharacterName = reader.ReadString(16);
            this.MaskedPassword = reader.ReadString(16);
            this.Body = reader.ReadUInt16();
            this.Class = reader.ReadUInt16();
            this.Token = reader.ReadUInt32();
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
            // Validate that the player has access to character creation
            if (client.Creation == null || this.Token != client.Creation.Token || 
                !Kernel.Registration.Contains(this.Token))
            {
                client.Send(new MsgTalk(this.Token, TalkChannel.Create, "Access Denied"));
                client.Socket.Disconnect(false);
                return;
            }

            // Check character name availability
            if (CharactersRepository.Exists(this.CharacterName))
            {
                client.Send(new MsgTalk(
                    this.Token, TalkChannel.Create, 
                    "Character name taken."));
                return;
            }

            // Validate character creation input
            if (!Enum.IsDefined(typeof(BodyType), this.Body) ||
                !Enum.IsDefined(typeof(BaseClassType), this.Class))
            {
                client.Send(new MsgTalk(
                    this.Token, TalkChannel.Create, 
                    "Invalid class or body type."));
                return;
            }

            // Create the character
            var character = new Character();
            character.AccountID = client.Creation.AccountID;
            character.Name = this.CharacterName;
            character.CurrentClass = (byte)this.Class;
            character.Mesh = this.Body;
            character.Hairstyle = 535;
            character.Silver = 1000;
            character.Level = 1;
            character.MapID = 1002;
            character.X = 430;
            character.Y = 380;
            character.Strength = 4;
            character.Agility = 6;
            character.Vitality = 12;
            character.Spirit = 0;
            character.HealthPoints = 12;
            character.Registered = DateTime.Now;

            try 
            { 
                // Save the character and continue with login
                CharactersRepository.Create(character); 
                Kernel.Registration.Remove(client.Creation.Token);
                client.Send(new MsgTalk(0, TalkChannel.Create, MsgTalk.ANSWEROK));
            }
            catch 
            { 
                client.Send(new MsgTalk(0, TalkChannel.Create, "Error, please try later")); 
            }
        }
    }
}