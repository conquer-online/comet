namespace Comet.Network.Packets
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Comet.Network.Sockets;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Packet processor for handling packets in background tasks using unbounded 
    /// channel. Allows for multiple writers, such as each remote client's accepted socket
    /// receive loop, to write to an assigned channel. Each reader has an associated 
    /// channel to guarantee client packet processing order.
    /// </summary>
    /// <typeparam name="TClient">Type of client being processed with the packet</typeparam>
    public class PacketProcessor<TClient> : BackgroundService
        where TClient : TcpServerActor
    {
        // Fields and Properties
        protected readonly Task[] BackgroundTasks;
        protected readonly Channel<Message>[] Channels;
        protected readonly Partition[] Partitions;
        protected readonly Func<TClient, byte[], Task> Process;
        protected CancellationToken CancelReads;
        protected CancellationToken CancelWrites;

        /// <summary>
        /// Instantiates a new instance of <see cref="PacketProcessor"/> using a default
        /// amount of worker tasks to initialize. Tasks will not be started.
        /// </summary>
        /// <param name="process">Processing task for channel messages</param>
        /// <param name="count">Number of threads to be created</param>
        public PacketProcessor(
            Func<TClient, byte[], Task> process,
            int count = 0)
        {
            // Initialize the channels and tasks as parallel arrays
            count = count == 0 ? Environment.ProcessorCount : count;
            this.BackgroundTasks = new Task[count];
            this.CancelReads = new CancellationToken();
            this.CancelWrites = new CancellationToken();
            this.Channels = new Channel<Message>[count];
            this.Partitions = new Partition[count];
            this.Process = process;
        }

        /// <summary>
        /// Triggered when the application host is ready to execute background tasks for
        /// dequeuing and processing work from unbounded channels. Work is queued by a
        /// connected and assigned client.
        /// </summary>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (int i = 0; i < this.BackgroundTasks.Length; i++)
            {
                this.Partitions[i] = new Partition { ID = (uint)i };
                this.Channels[i] = Channel.CreateUnbounded<Message>();
                this.BackgroundTasks[i] = DequeueAsync(this.Channels[i]);
            }
            
            return Task.WhenAll(this.BackgroundTasks);
        }

        /// <summary>
        /// Queues work by writing to a message channel. Work is queued by a connected
        /// client, and dequeued by the server's packet processing worker tasks. Each
        /// work item contains a single packet to be processed.
        /// </summary>
        /// <param name="actor">Actor requesting packet processing</param>
        /// <param name="packet">Packet bytes to be processed</param>
        public void Queue(TClient actor, byte[] packet)
        {
            if (!this.CancelWrites.IsCancellationRequested)
                this.Channels[actor.Partition].Writer.TryWrite(new Message {
                    Actor = actor,
                    Packet = packet
                });

        }

        /// <summary>
        /// Dequeues work in a loop. For as long as the thread is running and work is 
        /// available, work will be dequeued and processed. After dequeuing a message,
        /// the packet processor's <see cref="Process"/> action will be called.
        /// </summary>
        /// <param name="channel">Channel to read messages from</param>
        protected async Task DequeueAsync(Channel<Message> channel)
        {
            while (!this.CancelReads.IsCancellationRequested)
            {
                var msg = await channel.Reader.ReadAsync(this.CancelReads);
                if (msg != null) 
                { 
                    await this.Process(msg.Actor, msg.Packet).ConfigureAwait(false); 
                }
            }
        }

        /// <summary>
        /// Triggered when the application host is stopping the background task with a
        /// graceful shutdown. Requests that writes into the channel stop, and then reads
        /// from the channel stop. 
        /// </summary>
        public new async Task StopAsync(CancellationToken cancellationToken)
        {
            this.CancelWrites = new CancellationToken(true);
            foreach (var channel in this.Channels)
                await channel.Reader.Completion;
            this.CancelReads = new CancellationToken(true);
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Selects a partition for the client actor based on partition weight. The
        /// partition with the least popluation will be chosen first. After selecting a
        /// partition, that partition's weight will be increased by one.
        /// </summary>
        public uint SelectPartition()
        {
            uint partition = this.Partitions.Aggregate((aggr, next) => 
                next.Weight.CompareTo(aggr.Weight) < 0 ? next : aggr).ID;
            Interlocked.Increment(ref this.Partitions[partition].Weight);
            return partition;
        }

        /// <summary>
        /// Deslects a partition after the client actor disconnects.
        /// </summary>
        /// <param name="partition">The partition id to reduce the weight of</param>
        public void DeselectPartition(uint partition)
        {
            Interlocked.Decrement(ref this.Partitions[partition].Weight);
        }

        /// <summary>
        /// Defines a message for the <see cref="PacketProcessor"/>'s unbounded channel
        /// for queuing packets and actors requesting work. Each message defines a single
        /// unit of work - a single packet for processing. 
        /// </summary>
        protected class Message
        {
            public TClient Actor;
            public byte[] Packet;
        }

        /// <summary>
        /// Defines a partition for the <see cref="PacketProcessor"/>. This allows the 
        /// background service to track partition weight and assign clients to less 
        /// populated partitions. 
        /// </summary>
        protected class Partition
        {
            public uint ID;
            public int Weight;
        }
    }
}