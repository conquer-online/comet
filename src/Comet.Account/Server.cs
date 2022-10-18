namespace Comet.Account
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Comet.Account.Database;
    using Comet.Account.Packets;
    using Comet.Account.States;
    using Comet.Network.Packets;
    using Comet.Network.Sockets;

    /// <summary>
    /// Server inherits from a base server listener to provide the account server with
    /// listening functionality and event handling. This class defines how the server 
    /// listener and invoked events are customized for the account server.
    /// </summary>
    internal sealed class Server : TcpServerListener<Client>
    {
        // Fields and Properties
        private readonly PacketProcessor<Client> Processor;
        private readonly Task ProcessorTask;

        /// <summary>
        /// Instantiates a new instance of <see cref="Server"/> by initializing the 
        /// <see cref="PacketProcessor"/> for processing packets from the players using 
        /// channels and worker threads. Initializes the TCP server listener.
        /// </summary>
        /// <param name="config">The server's read configuration file</param>
        public Server(ServerConfiguration config) : base(maxConn: config.Network.MaxConn)
        {
            this.Processor = new PacketProcessor<Client>(this.ProcessAsync);
            this.ProcessorTask = this.Processor.StartAsync(CancellationToken.None);
        }

        /// <summary>
        /// Invoked by the server listener's Accepting method to create a new server actor
        /// around the accepted client socket. Gives the server an opportunity to initialize
        /// any processing mechanisms or authentication routines for the client connection.
        /// </summary>
        /// <param name="socket">Accepted client socket from the server socket</param>
        /// <param name="buffer">Preallocated buffer from the server listener</param>
        /// <returns>A new instance of a ServerActor around the client socket</returns>
        protected override async Task<Client> AcceptedAsync(Socket socket, Memory<byte> buffer)
        {
            // Interface must specify a task returning Accept method to support MsgEncryptCode
            // in future patches and the DH Key Exchange request on the game server.
            
            uint partition = this.Processor.SelectPartition();
            return await Task.FromResult(new Client(socket, buffer, partition));
        }

        /// <summary>
        /// Invoked by the server listener's Receiving method to process a completed packet
        /// from the actor's socket pipe. At this point, the packet has been assembled and
        /// split off from the rest of the buffer.
        /// </summary>
        /// <param name="actor">Server actor that represents the remote client</param>
        /// <param name="packet">Packet bytes to be processed</param>
        protected override void Received(Client actor, ReadOnlySpan<byte> packet)
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
        private async Task ProcessAsync(Client actor, byte[] packet) 
        {
            // Validate connection
            if (!actor.Socket.Connected)
                return;

            // Read in TQ's binary header
            var length = BitConverter.ToUInt16(packet, 0);
            var type = (PacketType)BitConverter.ToUInt16(packet, 2);

            try
            {
                // Switch on the packet type
                MsgBase<Client> msg = null;
                switch (type)
                {
                    case PacketType.MsgAccount: msg = new MsgAccount(); break;

                    default:
                        Console.WriteLine(
                            "Missing packet {0}, Length {1}\n{2}", 
                            type, length, PacketDump.Hex(packet));
                    return;
                }

                // Decode packet bytes into the structure and process
                msg.Decode(packet);
                await msg.ProcessAsync(actor);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
