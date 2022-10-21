namespace Comet.Game
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Comet.Game.Database;
    using Comet.Game.Packets;
    using Comet.Game.States;
    using Comet.Network.Packets;
    using Comet.Network.Security;
    using Comet.Network.Sockets;

    /// <summary>
    /// Server inherits from a base server listener to provide the game server with
    /// listening functionality and event handling. This class defines how the server 
    /// listener and invoked events are customized for the game server.
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
        public Server(ServerConfiguration config) 
            : base(maxConn: config.GameNetwork.MaxConn, exchange: true, footerLength: 8)
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
            var partition = this.Processor.SelectPartition();
            var client = new Client(socket, buffer, partition);
            var tasks = new List<Task>();

            tasks.Add(client.DiffieHellman.ComputePublicKeyAsync());
            tasks.Add(Kernel.NextBytesAsync(client.DiffieHellman.DecryptionIV));
            tasks.Add(Kernel.NextBytesAsync(client.DiffieHellman.EncryptionIV));
            await Task.WhenAll(tasks);

            var handshakeRequest = new MsgHandshake(
                client.DiffieHellman, 
                client.DiffieHellman.EncryptionIV,
                client.DiffieHellman.DecryptionIV);

            await handshakeRequest.RandomizeAsync();
            await client.SendAsync(handshakeRequest);
            return client;
        }

        /// <summary>
        /// Invoked by the server listener's Exchanging method to process the client 
        /// response from the Diffie-Hellman Key Exchange. At this point, the raw buffer 
        /// from the client has been decrypted and is ready for direct processing.
        /// </summary>
        /// <param name="actor">Server actor that represents the remote client</param>
        /// <param name="buffer">Packet buffer to be processed</param>
        /// <returns>True if the exchange was successful.</returns>
        protected override bool Exchanged(Client actor, ReadOnlySpan<byte> buffer)
        {
            try
            {
                MsgHandshake msg = new MsgHandshake();
                msg.Decode(buffer.ToArray());

                actor.DiffieHellman.ComputePrivateKey(msg.ClientKey);

                actor.Cipher.GenerateKeys(new object[] { 
                    actor.DiffieHellman.PrivateKey.ToByteArrayUnsigned() });
                (actor.Cipher as BlowfishCipher).SetIVs(
                    actor.DiffieHellman.DecryptionIV, 
                    actor.DiffieHellman.EncryptionIV);

                actor.DiffieHellman = null;
                return true;
            }
            catch (Exception e) { Console.WriteLine(e); }
            return false;
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
            PacketType type = (PacketType)BitConverter.ToUInt16(packet, 2);

            try
            {
                // Switch on the packet type
                MsgBase<Client> msg = null;
                switch (type)
                {
                    case PacketType.MsgRegister: msg = new MsgRegister(); break;
                    case PacketType.MsgItem:     msg = new MsgItem(); break;
                    case PacketType.MsgAction:   msg = new MsgAction(); break;
                    case PacketType.MsgConnect:  msg = new MsgConnect(); break;

                    default:
                        Console.WriteLine(
                            "Missing packet {0}, Length {1}\n{2}", 
                            type, length, PacketDump.Hex(packet));
                        await actor.SendAsync(new MsgTalk(actor.ID, MsgTalk.TalkChannel.Service,
                            String.Format("Missing packet {0}, Length {1}",
                            type, length)));
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

        /// <summary>
        /// Invoked by the server listener's Disconnecting method to dispose of the actor's
        /// resources. Gives the server an opportunity to cleanup references to the actor
        /// from other actors and server collections.
        /// </summary>
        /// <param name="actor">Server actor that represents the remote client</param>
        protected override void Disconnected(Client actor) 
        {
            if (actor == null) return;
            this.Processor.DeselectPartition(actor.Partition);
            Kernel.Clients.TryRemove(actor.ID, out _);

            if (actor.Creation != null)
                Kernel.Registration.Remove(actor.Creation.Token);

            if (actor.Character != null)
                actor.Character.SaveAsync(true).GetAwaiter();
        }
    }
}
