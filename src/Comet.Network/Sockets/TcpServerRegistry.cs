namespace Comet.Network.Sockets
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// TcpServerRegistry gives the server basic flood protection by keeping a registry of
    /// connection attempts, blocked connections, and active connections. A background worker
    /// will clean blocked records automatically.
    /// /// </summary>
    public sealed class TcpServerRegistry : IHostedService, IDisposable
    {
        // Fields and properties
        private Dictionary<string, int> Active;
        private Dictionary<string, int> Attempts;
        private ConcurrentDictionary<string, DateTime> Blocks;
        private Timer PurgeTimer;
        private object ActiveMutex;
        private object AttemptsMutex;
        private readonly int BanMinutes;
        private readonly int MaxActiveConnections;
        private readonly int MaxAttemptsPerMinute;

        /// <summary>
        /// Instantiates a new instance of <see cref="TcpServerRegistry"/> with initialized
        /// collections for connection registration checks. The background worker for
        /// trimming connections does not start until Start is called.
        /// </summary>
        /// <param name="banMinutes">Minutes a ban should remain in effect for</param>
        /// <param name="maxActiveConn">Maximum active connections alive at any given time</param>
        /// <param name="maxAttemptsPerMinute">Maximum connection attempts per minute</param>
        public TcpServerRegistry(
            int banMinutes = 15,
            int maxActiveConn = 10, 
            int maxAttemptsPerMinute = 15)
        {
            this.Active = new Dictionary<string, int>();
            this.Attempts = new Dictionary<string, int>();
            this.Blocks = new ConcurrentDictionary<string, DateTime>();

            this.ActiveMutex = new object();
            this.AttemptsMutex = new object();
            this.BanMinutes = banMinutes;
            this.MaxActiveConnections = maxActiveConn;
            this.MaxAttemptsPerMinute = maxAttemptsPerMinute;
        }

        /// <summary>
        /// Triggered when the application host is ready to start cleaning connection records
        /// from the registry. Blocked entries with expired times will be unblocked, and
        /// attempt counters will be reset.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.PurgeTimer = new Timer(
                this.TimedPurgeJob, 
                null, 
                TimeSpan.Zero, 
                TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        /// <summary>Stops cleaning attempt counters and stop checking for bans.</summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.PurgeTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>Disposes of the purge timer.</summary>
        public void Dispose()
        {
            this.PurgeTimer?.Dispose();
        }

        /// <summary>
        /// Adds a new active client to the registry of connections. If the maximum number 
        /// of active connections has been exceeded for an IP address, or the accept volume
        /// has spiked beyond permitted limits, this method will return false and ban the
        /// client if evaluated to be an attack.
        /// </summary>
        /// <param name="ip">IPv4 address of the client</param>
        /// <returns>True if the connection is allowed.</returns>
        public bool AddActiveClient(string ip)
        {
            // Check for blocked IP addresses
            if (this.Blocks.ContainsKey(ip))
            {
                return false;
            }

            // Check if the client should be blocked for frequent connections and then 
            // increment the active connections counter if the previous operation succeeded.
            return IncrementCounter(ip, this.MaxAttemptsPerMinute, ref this.AttemptsMutex, ref this.Attempts)
                && IncrementCounter(ip, this.MaxActiveConnections, ref this.ActiveMutex, ref this.Active);
        }

        /// <summary>
        /// Increments a counter given a collection of counts keyed by the client's IP 
        /// address. If the counter exceeds the ceiling value set by the parent method, then
        /// the connection will be banned.
        /// </summary>
        /// <param name="ip">IPv4 address of the client</param>
        /// <param name="ceiling">Highest counter value before banning the connection</param>
        /// <param name="mutex">Mutex for locking the counter collection</param>
        /// <param name="collection">Counter collection keyed by IP address</param>
        /// <returns>True if the counter was incremented and the client wasn't banned.</returns>
        public bool IncrementCounter(
            string ip,
            int ceiling,
            ref object mutex,
            ref Dictionary<string, int> collection)
        {
            lock (mutex)
            if (collection.TryGetValue(ip, out int count))
            {
                count++;
                if (count > this.MaxActiveConnections)
                {
                    this.Blocks.TryAdd(ip, DateTime.UtcNow.AddMinutes(this.BanMinutes));
                    return false;
                }

                collection[ip] = count;
            }
            else collection.TryAdd(ip, 1);
            return true;
        }

        /// <summary>Removes an active connection from the registry.</summary>
        /// <param name="ip">IPv4 address of the client</param>
        public void RemoveActiveClient(string ip)
        {
            // Decrement active connections count
            lock (this.ActiveMutex)
            if (this.Active.TryGetValue(ip, out int attempts))
            {
                attempts--;
                if (attempts == 0) 
                {
                    this.Active.Remove(ip);
                    return;
                }
                else this.Active[ip] = attempts;
            }
        }

        /// <summary>
        /// Invoked on a timer to purge attempt counters and check for expired bans. If the
        /// interval calling this method isn't fast enough, it could cause players to get
        /// banned unjustly.
        /// </summary>
        public void TimedPurgeJob(object state)
        {
            lock (AttemptsMutex) 
                this.Attempts.Clear();

            DateTime now = DateTime.UtcNow;
            foreach (var blockedConnection in this.Blocks)
            {
                if (blockedConnection.Value < now)
                    this.Blocks.TryRemove(blockedConnection.Key, out DateTime _);
            }
        }
    }
}
