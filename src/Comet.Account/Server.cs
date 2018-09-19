namespace Comet.Account
{
    using System;
    using System.Net.Sockets;
    using Comet.Account.States;
    using Comet.Network.Packets;
    using Comet.Network.Security;
    using Comet.Network.Sockets;

    /// <summary>
    /// Server inherits from a base server listener to provide the account server with
    /// listening functionality and event handling. This class defines how the server 
    /// listener and invoked events are customized for the account server.
    /// </summary>
    internal sealed class Server : TcpServerListener
    {
        // Fields and Properties
        private readonly PacketProcessor Processor;

        /// <summary>
        /// Instantiates a new instance of <see cref="Server"/> by initializing the 
        /// <see cref="PacketProcessor"/> for processing packets from the players using 
        /// channels and worker threads. Initializes the TCP server listener.
        /// </summary>
        /// <param name="config">The server's read configuration file</param>
        public Server(ServerConfiguration config) : base(maxConn: config.Network.MaxConn)
        {
            this.Processor = new PacketProcessor(this.Process);
        }

        /// <summary>
        /// Invoked by the server listener's Accepting method to create a new server actor
        /// around the accepted client socket. Gives the server an opportunity to initialize
        /// any processing mechanisms or authentication routines for the client connection.
        /// </summary>
        /// <param name="socket">Accepted client socket from the server socket</param>
        /// <param name="buffer">Preallocated buffer from the server listener</param>
        /// <returns>A new instance of a ServerActor around the client socket</returns>
        protected override TcpServerActor Accepted(Socket socket, Memory<byte> buffer)
        {
            return new Client(socket, buffer);
        }

        /// <summary>
        /// Invoked by the server listener's Receiving method to process a completed packet
        /// from the actor's socket pipe. At this point, the packet has been assembled and
        /// split off from the rest of the buffer.
        /// </summary>
        /// <param name="actor">Server actor that represents the remote client</param>
        /// <param name="packet">Packet bytes to be processed</param>
        protected override void Received(TcpServerActor actor, ReadOnlySpan<byte> packet)
        {
            this.Processor.Queue(actor, packet.ToArray());
        }

        /// <summary>
        /// Invoked by one of the server's packet processor worker threads to process a
        /// single packet of work. Allows the server to process packets as individual 
        /// messages on a single channel.
        /// </summary>
        /// <param name="actor">Actor requesting packet processing</param>
        /// <param name="packet">An individual data packet to be processed</param>
        private void Process(TcpServerActor actor, byte[] packet) 
        {
            Console.WriteLine("Processing {0} bytes", packet.Length);
            Console.WriteLine(PacketDump.Hex(packet));
        }
    }
}
