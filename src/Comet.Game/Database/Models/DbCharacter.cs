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
    public partial class DbCharacter
    {
        // Column Properties
        public virtual uint CharacterID { get; set; }
        public virtual uint AccountID { get; set; }
        public virtual string Name { get; set; }
        public virtual uint Mesh { get; set; }
        public virtual ushort Hairstyle { get; set; }
        public virtual uint Silver { get; set; }
        public virtual uint Jewels { get; set; }
        public virtual byte CurrentClass { get; set; }
        public virtual byte PreviousClass { get; set; }
        public virtual byte Rebirths { get; set; }
        public virtual byte Level { get; set; }
        public virtual ulong Experience { get; set; }
        public virtual uint MapID { get; set; }
        public virtual ushort X { get; set; }
        public virtual ushort Y { get; set; }
        public virtual uint Virtue { get; set; }
        public virtual ushort Strength { get; set; }
        public virtual ushort Agility { get; set; }
        public virtual ushort Vitality { get; set; }
        public virtual ushort Spirit { get; set; }
        public virtual ushort AttributePoints { get; set; }
        public virtual ushort HealthPoints { get; set; }
        public virtual ushort ManaPoints { get; set; }
        public virtual ushort KillPoints { get; set; }
        public virtual DateTime Registered { get; set; }
    }
}