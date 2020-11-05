namespace Comet.Network.Services
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// This background service instantiates a single Random instance to generate all random
    /// numbers for the server. This allows the server to generate random numbers across 
    /// multiple threads without generating the same number or returning zero. This service
    /// in particular buffers random numbers to a channel to avoid locking.
    /// </summary>
    public class RandomnessService : BackgroundService
    {
        // Fields and Properties
        private Channel<Double> BufferChannel;
        protected Random Generator;

        /// <summary>
        /// Instantiates a new instance of <see cref="RandomnessService"/> using a default
        /// capacity to buffer random numbers.
        /// </summary>
        /// <param name="capacity">Capacity of the bounded channel.</param>
        public RandomnessService(int capacity = 10000)
        {
            this.BufferChannel = Channel.CreateBounded<Double>(capacity);
            this.Generator = new Random();
        }

        /// <summary>
        /// Triggered when the application host is ready to start queuing random numbers.
        /// Since the channel holding random numbers is bounded, writes will block 
        /// naturally on an await rather than locking threads to generate numbers.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await this.BufferChannel.Writer.WriteAsync(
                    this.Generator.NextDouble(), 
                    stoppingToken);
            }
        }

        /// <summary>Returns the next random number from the generator.</summary>
        /// <param name="minValue">The least legal value for the Random number.</param>
        /// <param name="maxValue">One greater than the greatest legal return value.</param>
        public async Task<int> NextAsync(int minValue, int maxValue) 
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            var range = (long)maxValue - minValue;
            if (range > (long)Int32.MaxValue)
                throw new ArgumentOutOfRangeException();

            var value = await this.BufferChannel.Reader.ReadAsync();
            var result = ((int)(value * range) + minValue);
            return result;
        }

        /// <summary>Writes random numbers from the generator to a buffer.</summary>
        /// <param name="buffer">Buffer to write bytes to.</param>
        public async Task NextBytesAsync(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)(await this.NextAsync(0, 255));
        }
    }
}
