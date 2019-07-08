namespace Comet.Network.Sockets
{
    using System;
    using System.Buffers;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// TcpServerListener implements an asynchronous TCP streaming socket server for high
    /// performance server logic. Socket operations are processed in value tasks using
    /// Socket Task Extensions. Inherits from a base class for providing socket operation
    /// event handling to the non-abstract derived class of TcpServerListener.
    /// </summary>
    public abstract class TcpServerListener : TcpServerEvents
    {
        // Fields and properties
        private readonly Semaphore AcceptanceSemaphore;
        private readonly ConcurrentStack<Memory<byte>> BufferPool;
        private readonly TaskFactory ReceiveTasks;
        private readonly CancellationTokenSource ShutdownToken;
        private readonly Socket Socket;

        /// <summary>
        /// Instantiates a new instance of <see cref="TcpServerListener"/> with a new server
        /// socket for accepting remote or local client connections. Creates preallocated
        /// buffers for receiving data from clients without expensive allocations per receive
        /// operation.
        /// </summary>
        /// <param name="maxConn">Maximum number of clients connected</param>
        /// <param name="bufferSize">Preallocated buffer size in bytes</param>
        /// <param name="delay">Use Nagel's algorithm to delay sending smaller packets</param>
        public TcpServerListener(int maxConn = 500, int bufferSize = 4096, bool delay = false)
        {
            // Initialize and configure server socket
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Socket.LingerState = new LingerOption(false, 0);
            this.Socket.NoDelay = !delay;

            // Initialize management mechanisms
            this.AcceptanceSemaphore = new Semaphore(maxConn, maxConn);
            this.BufferPool = new ConcurrentStack<Memory<byte>>();
            this.ShutdownToken = new CancellationTokenSource();
            this.ReceiveTasks = new TaskFactory(this.ShutdownToken.Token);

            // Initialize preallocated buffer pool
            for (int i = 0; i < maxConn; i++)
                this.BufferPool.Push(new Memory<byte>(new byte[bufferSize]));
        }

        /// <summary>
        /// Binds the server listener to a port and network interface. Specify "0.0.0.0" 
        /// as the endpoint address to bind to all interfaces on the host machine. Starts
        /// the server listener and accepts new connections in a new task.
        /// </summary>
        /// <param name="port">Port number the server will bind to</param>
        /// <param name="address">Interface IPv4 address the server will bind to</param>
        /// <param name="backlog">Maximum connections backlogged for acceptance</param>
        /// <returns>Returns a new task for accepting new connections.</returns>
        public Task Start(int port, string address = "0.0.0.0", int backlog = 100)
        {
            this.Socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            this.Socket.Listen(backlog);
            return this.Accepting();
        }

        /// <summary>
        /// Accepting accepts client connections asynchronously as a new task. As a client
        /// connection is accepted, it will be associated with a preallocated buffer and
        /// a receive task. The accepted socket event will be called after accept.
        /// </summary>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task Accepting()
        {
            while (this.Socket.IsBound && !this.ShutdownToken.IsCancellationRequested)
            {
                // Block if the maximum connections has been reached. Holds all connection
                // attempts in the backlog of the server socket until a client disconnects
                // and a new client can be accepted. Check shutdown every 5 seconds.
                if (this.AcceptanceSemaphore.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    // Pop a preallocated buffer and accept a client
                    this.BufferPool.TryPop(out var buffer);
                    var socket = await this.Socket.AcceptAsync();
                    var actor = this.Accepted(socket, buffer);

                    // Start receiving data from the client connection
                    var task = this.ReceiveTasks
                        .StartNew(this.Receiving, actor, this.ShutdownToken.Token)
                        .ContinueWith(actor.ReceiveFault);
                }
            }
        }

        /// <summary>
        /// Receiving receives bytes from the accepted client socket when bytes become
        /// available. While the client is connected and the server hasn't issued the 
        /// shutdown signal, bytes will be received in a loop.
        /// </summary>
        /// <param name="state">Created actor around the accepted client socket</param>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task Receiving(object state)
        {
            // Initialize multiple receive variables
            var actor = state as TcpServerActor;
            int consumed = 0, examined = 0;
            while (actor.Socket.Connected && !this.ShutdownToken.IsCancellationRequested)
            {
                // Receive data from the client socket
                examined = await actor.Socket.ReceiveAsync(
                    actor.Buffer.Slice(examined - consumed), 
                    SocketFlags.None, 
                    this.ShutdownToken.Token);
                if (examined == 0) break;

                // Decrypt traffic
                actor.Cipher.Decrypt(
                    actor.Buffer.Slice(0, examined).Span, 
                    actor.Buffer.Slice(0, examined).Span);

                // Handle splitting and processing of data
                this.Splitting(actor, examined, ref consumed);
                actor.Buffer.Slice(consumed, examined).CopyTo(actor.Buffer);
            }

            // Disconnect the client
            this.Disconnecting(actor);
        }

        /// <summary>
        /// Splitting splits the actor's receive buffer into multiple packets that can
        /// then be processed by Received individually. The default behavior of this method
        /// unless otherwise overridden is to split packets from the buffer using an unsigned
        /// short packet header for the length of each packet.
        /// </summary>
        /// <param name="buffer">Actor for consuming bytes from the buffer</param>
        /// <param name="examined">Number of examined bytes from the receive</param>
        /// <param name="consumed">Number of consumed bytes by the split reader</param>
        protected virtual void Splitting(TcpServerActor actor, int examined, ref int consumed)
        {
            // Consume packets from the socket buffer
            consumed = 0;
            var buffer = actor.Buffer.Span;
            while (consumed + 2 < examined)
            {
                var length = BitConverter.ToUInt16(buffer.Slice(consumed, 2));
                if (consumed + length > examined) break;
                this.Received(actor, buffer.Slice(consumed, length));
                consumed += length;
            }
        }

        /// <summary>
        /// Disconnecting is called when the client is disconnecting from the server. Allows
        /// the server to handle client events post-disconnect, and reclaim resources first
        /// leased to the client on accept.
        /// </summary>
        /// <param name="actor">Actor being disconnected</param>
        private void Disconnecting(TcpServerActor actor)
        {
            // Reclaim resources and release back to server pools
            actor.Buffer.Span.Clear();
            this.BufferPool.Push(actor.Buffer);
            this.AcceptanceSemaphore.Release();
            
            // Complete processing for disconnect
            this.Disconnected(actor);
        }
    }
}
