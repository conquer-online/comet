namespace Comet.Core.Mathematics
{
    /// <summary>
    /// This class contains extension methods for providing unsigned integers with bitwise
    /// rotation / circular shift functions. Rotations in this implementation follow the C
    /// compiler standard for ANSI C language instructions for rotations (32-bit rotations). 
    /// </summary>
    public static class Rotations
    {
        /// <summary>Performs a bitwise left rotation.</summary>
        /// <param name="value">Current integer to be rotated</param>
        /// <param name="count">Number of bytes to be shifted by</param>
        /// <returns>Returns the resulting rotation.</returns>
        public static uint RotateLeft(this uint value, int count) =>
            (value << count % 32) | (value >> (32 - count % 32));
        
        /// <summary>Performs a bitwise right rotation.</summary>
        /// <param name="value">Current integer to be rotated</param>
        /// <param name="count">Number of bytes to be shifted by</param>
        /// <returns>Returns the resulting rotation.</returns>
        public static uint RotateRight(this uint value, int count) =>
            (value >> count % 32) | (value << (32 - count % 32));
    }
}
