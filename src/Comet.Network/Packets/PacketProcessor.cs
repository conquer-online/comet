namespace Comet.Network.Packets
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Comet.Network.Sockets;

    /// <summary>
    /// Packet processor for handling packets in worker threads using unbounded channels.
    /// Allows for multiple writers, such as each remote client's accepted socket receive
    /// loop, to write to the channel. Multiple readers then read single messages from
    /// the channel for processing.
    /// </summary>
    public class PacketProcessor
    {
        // Fields and Properties
        protected readonly Channel<Message> Messages;
        protected readonly Action<TcpServerActor, byte[]> Process;
        protected readonly List<Task> Tasks;
        protected CancellationToken CancelReads;
        protected CancellationToken CancelWrites;

        /// <summary>
        /// Instantiates a new instance of <see cref="PacketProcessor"/> using a default
        /// amount of worker threads to initialize. Creates threads and starts threads
        /// immediately to read data from the unbounded channel.
        /// </summary>
        /// <param name="process">Processing action for channel messages</param>
        /// <param name="cancelReads">Token for cancelling reads</param>
        /// <param name="cancelWrites">Token for cancelling writes</param>
        /// <param name="count">Number of threads to be created</param>
        public PacketProcessor(
            Action<TcpServerActor, byte[]> process, 
            CancellationToken cancelReads = default(CancellationToken),
            CancellationToken cancelWrites = default(CancellationToken),
            int count = 0)
        {
            // Initialize the channel
            this.CancelReads = cancelReads;
            this.CancelWrites = cancelWrites;
            this.Messages = Channel.CreateUnbounded<Message>();
            this.Process = process;
            this.Tasks = new List<Task>();

            // Create threads for reading from the channel
            count = count == 0 ? Environment.ProcessorCount : count;
            for (int i = 0; i < count; i++)
                this.Tasks.Add(this.Dequeue());
        }

        /// <summary>
        /// Queues work by writing to the messages channel. Work is queued by a connected
        /// client, and dequeued by the server's packet processing worker threads. Each
        /// work item contains a single packet to be processed.
        /// </summary>
        /// <param name="actor">Actor requesting packet processing</param>
        /// <param name="packet">Packet bytes to be processed</param>
        public void Queue(TcpServerActor actor, byte[] packet)
        {
            if (!this.CancelWrites.IsCancellationRequested)
                this.Messages.Writer.TryWrite(new Message {
                    Actor = actor,
                    Packet = packet
                });
        }

        /// <summary>
        /// Dequeues work in a loop. For as long as the thread is running and work is 
        /// available, work will be dequeued and processed. After dequeuing a message,
        /// the packet processor's <see cref="Process"/> action will be called. 
        /// </summary>
        private async Task Dequeue()
        {
            while (!this.CancelReads.IsCancellationRequested)
            {
                var msg = await this.Messages.Reader.ReadAsync(this.CancelReads);
                this.Process(msg.Actor, msg.Packet);
            }
        }

        /// <summary>
        /// Shuts down reads and then writes for the packet processor, allowing for the
        /// processing channel to be completely processed before shutting down threads.
        /// </summary>
        public void Shutdown()
        {
            this.CancelWrites = new CancellationToken(true);
            this.Messages.Reader.Completion.Wait();
            this.CancelReads = new CancellationToken(true);
        }
        
        /// <summary>
        /// Defines a message for the <see cref="PacketProcessor"/>'s unbounded channel
        /// for queuing packets and actors requesting work. Each message defines a single
        /// unit of work - a single packet for processing. 
        /// </summary>
        protected class Message
        {
            public TcpServerActor Actor;
            public byte[] Packet;
        }
    }
}
