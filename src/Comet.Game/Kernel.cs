namespace Comet.Game
{
    using Comet.Game.States;
    using Comet.Network.Services;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Runtime.Caching;
    using System.Threading.Tasks;

    /// <summary>
    /// Kernel for the server, acting as a central core for pools of models and states
    /// initialized by the server. Used in database repositories to load data into memory
    /// from essential tables or tables which require heavy post-processing. Used in the
    /// server packet process methods for tracking client and world states.
    /// </summary>
    public static class Kernel
    {
        // State caches
        public static ConcurrentDictionary<uint, Client> Clients = new ConcurrentDictionary<uint, Client>();
        public static MemoryCache Logins = MemoryCache.Default;
        public static List<uint> Registration = new List<uint>();

        // Background services
        public static class Services
        {
            public static RandomnessService Randomness = new RandomnessService();
        }

        /// <summary>Returns the next random number from the generator.</summary>
        /// <param name="minValue">The least legal value for the Random number.</param>
        /// <param name="maxValue">One greater than the greatest legal return value.</param>
        public static Task<int> NextAsync(int minValue, int maxValue) => 
            Services.Randomness.NextAsync(minValue, maxValue);

        /// <summary>Writes random numbers from the generator to a buffer.</summary>
        /// <param name="buffer">Buffer to write bytes to.</param>
        public static Task NextBytesAsync(byte[] buffer) =>
            Services.Randomness.NextBytesAsync(buffer);
    }
}