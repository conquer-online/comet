namespace Comet.Game.States
{
    /// <summary>
    /// Creation holds the current state of the character creation process. The class is
    /// initialized once on client connect if a character for the player doesn't exist.
    /// The class is uninitialized on disconnect or when the player's character has been
    /// created successfully.
    /// </summary>
    public sealed class Creation
    {
        public uint AccountID;
        public uint Token;
    }
}