namespace Comet.Game.Packets
{
    using System.Collections.Generic;
    using System.IO;
    using Comet.Game.Database.Models;
    using Comet.Game.States;
    using Comet.Network.Packets;

    /// <remarks>Packet Type 1006</remarks>
    /// <summary>
    /// Message defining character information, used to initialize the client interface
    /// and game state. Character information is loaded from the game database on login
    /// if a character exists.
    /// </summary>
    public sealed class MsgUserInfo : MsgBase<Client>
    {
        // Packet Properties
        public uint CharacterID { get; set; }
        public uint Mesh { get; set; }
        public ushort Hairstyle { get; set; }
        public uint Silver { get; set; }
        public uint Jewels { get; set; }
        public ulong Experience { get; set; }
        public ushort Strength { get; set; }
        public ushort Agility { get; set; }
        public ushort Vitality { get; set; }
        public ushort Spirit { get; set; }
        public ushort AttributePoints { get; set; }
        public ushort HealthPoints { get; set; }
        public ushort ManaPoints { get; set; }
        public ushort KillPoints { get; set; }
        public byte Level { get; set; }
        public byte CurrentClass { get; set; }
        public byte PreviousClass { get; set; }
        public byte Rebirths { get; set; }
        public byte AncestorClass { get; set; }
        public bool HasName { get; set; }
        public string CharacterName { get; set; }
        public string SpouseName { get; set; }

        /// <summary>
        /// Instantiates a new instance of <see cref="MsgUserInfo"/> using data fetched
        /// from the database and stored in <see cref="DbCharacter"/>.
        /// </summary>
        /// <param name="character">Character info from the database</param>
        public MsgUserInfo(DbCharacter character)
        {
            base.Type = PacketType.MsgUserInfo;
            this.CharacterID = character.CharacterID;
            this.Mesh = (uint)(character.Mesh + (character.Avatar * 10000));
            this.Hairstyle = character.Hairstyle;
            this.Silver = character.Silver;
            this.Jewels = character.Jewels;
            this.Experience = character.Experience;
            this.Strength = character.Strength;
            this.Agility = character.Agility;
            this.Vitality = character.Vitality;
            this.Spirit = character.Spirit;
            this.AttributePoints = character.AttributePoints;
            this.HealthPoints = character.HealthPoints;
            this.ManaPoints = character.ManaPoints;
            this.KillPoints = character.KillPoints;
            this.Level = character.Level;
            this.CurrentClass = character.CurrentClass;
            this.PreviousClass = character.PreviousClass;
            this.AncestorClass = character.AncestorClass;
            this.Rebirths = character.Rebirths;
            this.CharacterName = character.Name;
            this.SpouseName = "None";
            this.HasName = true;
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
            writer.Write(this.Mesh);
            writer.Write(this.Hairstyle);
            writer.Write(this.Silver);
            writer.Write(this.Jewels);
            writer.Write(this.Experience);
            writer.Write((ulong)0);
            writer.Write((ulong)0);
            writer.Write((uint)0);
            writer.Write(this.Strength);
            writer.Write(this.Agility);
            writer.Write(this.Vitality);
            writer.Write(this.Spirit);
            writer.Write(this.AttributePoints);
            writer.Write(this.HealthPoints);
            writer.Write(this.ManaPoints);
            writer.Write(this.KillPoints);
            writer.Write(this.Level);
            writer.Write(this.CurrentClass);
            writer.Write(this.PreviousClass);
            writer.Write(this.Rebirths);
            writer.Write(this.HasName);
            writer.Write(new List<string>{
                this.CharacterName,
                this.SpouseName
            });
            return writer.ToArray();
        }
    }
}