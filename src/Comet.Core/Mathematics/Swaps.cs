namespace Comet.Core.Mathematics
{
    using System;

    /// <summary>
    /// This class contains extension methods for providing unsigned integers with swap
    /// functions for swapping values between integers.
    /// </summary>
    public static class Swaps
    {
        /// <summary>Swaps the values of two unsigned integers.</summary>
        /// <param name="n1">The first number</param>
        /// <param name="n2">The first number</param>
        public static void Swap(this ref uint n1, ref uint n2)
        {
            uint tmp = n1;
            n1 = n2;
            n2 = tmp;
        }
    }
}
