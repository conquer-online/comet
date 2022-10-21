namespace Comet.Network.Sockets
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Comet.Network.Packets;
    using Comet.Network.Security;

    /// <summary>
    /// Actors are assigned to accepted client sockets to give connected clients a state
    /// across socket operations. This allows the server to handle multiple receive writes
    /// across single processing reads, and keep a buffer alive for faster operations.
    /// </summary>
    public abstract class TcpServerActor
    {
        // Fields and Properties
        public readonly Memory<byte> Buffer;
        public readonly ICipher Cipher;
        public readonly Socket Socket;
        public readonly string IPAddress;
        public readonly byte[] PacketFooter;
        public readonly uint Partition;
        private readonly object SendLock;

        /// <summary>
        /// Instantiates a new instance of <see cref="TcpServerActor"/> using an accepted
        /// client socket and preallocated buffer from the server listener.
        /// </summary>
        /// <param name="socket">Accepted client socket</param>
        /// <param name="buffer">Preallocated buffer for socket receive operations</param>
        /// <param name="cipher">Cipher for handling client encipher operations</param>
        /// <param name="partition">Packet processing partition, default is disabled</param>
        /// <param name="packetFooter">Length of the packet footer</param>
        public TcpServerActor(
            Socket socket, 
            Memory<byte> buffer, 
            ICipher cipher, 
            uint partition = 0,
            string packetFooter = "")
        {
            this.Buffer = buffer;
            this.Cipher = cipher;
            this.Socket = socket;
            this.IPAddress = (socket.RemoteEndPoint as IPEndPoint).Address.MapToIPv4().ToString();
            this.PacketFooter = Encoding.ASCII.GetBytes(packetFooter);
            this.Partition = partition;
            this.SendLock = new object();
        }

        /// <summary>
        /// Sends a packet to the game client after encrypting bytes. This may be called
        /// as-is, or overridden to provide channel functionality and thread-safety around
        /// the accepted client socket. By default, this method locks around encryption
        /// and sending data. 
        /// </summary>
        /// <param name="packet">Bytes to be encrypted and sent to the client</param>
        public virtual Task SendAsync(byte[] packet)
        {
            var encrypted = new byte[packet.Length + this.PacketFooter.Length];
            packet.CopyTo(encrypted, 0);

            BitConverter.TryWriteBytes(encrypted, (ushort)packet.Length);
            Array.Copy(this.PacketFooter, 0, encrypted, packet.Length, this.PacketFooter.Length);

            lock (SendLock)
            {
                try 
                {
                    this.Cipher.Encrypt(encrypted, encrypted);
                    this.Socket?.Send(encrypted, SocketFlags.None);
                    return Task.CompletedTask;
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode < SocketError.ConnectionAborted ||
                        e.SocketErrorCode > SocketError.Shutdown)
                        Console.WriteLine(e);
                    return Task.FromException(e);
                }
            }
        }

        /// <summary>
        /// Sends a packet to the game client after encrypting bytes. This may be called
        /// as-is, or overridden to provide channel functionality and thread-safety around
        /// the accepted client socket. By default, this method locks around encryption
        /// and sending data. 
        /// </summary>
        /// <param name="packet">Packet to be encrypted and sent to the client</param>
        public virtual async Task SendAsync(IPacket packet)
        {
            await this.SendAsync(packet.Encode());
        }

        /// <summary>
        /// Force closes the client connection.
        /// </summary>
        public virtual void Disconnect()
        {
            this.Socket?.Disconnect(false);
        }
    }    
}
