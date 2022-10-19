namespace Comet.Game.States
{
    using System;
    using System.Threading.Tasks;
    using Comet.Game.Database.Models;
    using Comet.Game.Database.Repositories;

    /// <summary>
    /// Character class defines a database record for a player's character. This allows
    /// for easy saving of character information, as well as means for wrapping character
    /// data for spawn packet maintenance, interface update pushes, etc.
    /// </summary>
    public sealed class Character : DbCharacter
    {
        // Fields and properties
        private DateTime LastSaveTimestamp { get; set; }

        /// <summary>
        /// Instantiates a new instance of <see cref="Character"/> using a database fetched
        /// <see cref="DbCharacter"/>. Copies attributes over to the base class of this
        /// class, which will then be used to save the character from the game world. 
        /// </summary>
        /// <param name="character">Database character information</param>
        public Character(DbCharacter character) 
            : base(character.AccountID, character.CharacterID, character.Name)
        {
            base.Agility = character.Agility;
            base.AncestorClass = character.AncestorClass;
            base.AttributePoints = character.AttributePoints;
            base.Avatar = character.Avatar;
            base.CurrentClass = character.CurrentClass;
            base.Experience = character.Experience;
            base.Hairstyle = character.Hairstyle;
            base.HealthPoints = character.HealthPoints;
            base.Jewels = character.Jewels;
            base.KillPoints = character.KillPoints;
            base.Level = character.Level;
            base.ManaPoints = character.ManaPoints;
            base.MapID = character.MapID;
            base.Mesh = character.Mesh;
            base.PreviousClass = character.PreviousClass;
            base.Rebirths = character.Rebirths;
            base.Registered = character.Registered;
            base.Silver = character.Silver;
            base.Spirit = character.Spirit;
            base.Strength = character.Strength;
            base.Virtue = character.Virtue;
            base.Vitality = character.Vitality;
            base.X = character.X;
            base.Y = character.Y;

            // Initialize local properties
            this.LastSaveTimestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Saves the character to persistent storage.
        /// </summary>
        /// <param name="force">True if the change is important to save immediately.</param>
        public async Task SaveAsync(bool force = false)
        {
            DateTime now = DateTime.UtcNow;
            if (force || this.LastSaveTimestamp.AddMilliseconds(CharactersRepository.ThrottleMilliseconds) < now)
            {
                this.LastSaveTimestamp = now;
                await CharactersRepository.SaveAsync(this);
            }
        }
    }

    /// <summary>Enumeration type for body types for player characters.</summary>
    public enum BodyType : ushort
    {
        AgileMale = 1003,
        MuscularMale = 1004,
        AgileFemale = 2001,
        MuscularFemale = 2002
    }

    /// <summary>Enumeration type for base classes for player characters.</summary>
    public enum BaseClassType : ushort
    {
        Trojan = 10,
        Warrior = 20,
        Archer = 40,
        Taoist = 100
    }
}