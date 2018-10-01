namespace Comet.Game.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Character information associated with a player. Every player account is permitted
    /// a single character on the server. Contains the character's defining look and features,
    /// level and attribute information, location, etc.
    /// </summary>
    [Table("character")]
    public partial class Character
    {
        // Column Properties
        public uint CharacterID { get; set; }
        public uint AccountID { get; set; }
        public string Name { get; set; }
        public uint Mesh { get; set; }
        public ushort Hairstyle { get; set; }
        public uint Silver { get; set; }
        public uint Jewels { get; set; }
        public byte CurrentClass { get; set; }
        public byte PreviousClass { get; set; }
        public byte Rebirths { get; set; }
        public byte Level { get; set; }
        public ulong Experience { get; set; }
        public uint MapID { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public uint Virtue { get; set; }
        public ushort Strength { get; set; }
        public ushort Agility { get; set; }
        public ushort Vitality { get; set; }
        public ushort Spirit { get; set; }
        public ushort AttributePoints { get; set; }
        public ushort HealthPoints { get; set; }
        public ushort ManaPoints { get; set; }
        public ushort KillPoints { get; set; }
        public DateTime Registered { get; set; }
    }
}