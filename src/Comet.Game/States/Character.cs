namespace Comet.Game.States
{
    using Comet.Game.Database.Models;

    /// <summary>
    /// Character class defines a database record for a player's character. This allows
    /// for easy saving of character information, as well as means for wrapping character
    /// data for spawn packet maintenance, interface update pushes, etc.
    /// </summary>
    public class Character : DbCharacter
    {
        
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