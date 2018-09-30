namespace Comet.Network.Sockets
{
    using System;
    using System.Net;
    using System.Net.Sockets;
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
        public readonly Socket Socket;
        public readonly ICipher Cipher;
        private readonly object SendLock;

        /// <summary>
        /// Instantiates a new instance of <see cref="TcpServerActor"/> using an accepted
        /// client socket and preallocated buffer from the server listener.
        /// </summary>
        /// <param name="socket">Accepted client socket</param>
        /// <param name="buffer">Preallocated buffer for socket receive operations</param>
        /// <param name="cipher">Cipher for handling client encipher operations</param>
        public TcpServerActor(Socket socket, Memory<byte> buffer, ICipher cipher)
        {
            this.Buffer = buffer;
            this.Socket = socket;
            this.Cipher = cipher;
            this.SendLock = new object();
        }

        // <summary>
        /// Allows the system to process faults from the receive task for a remote client
        /// connection. Gives the server the ability to perform a graceful shutdown or 
        /// receive retry, depending on the status of the connection and server.
        /// </summary>
        /// <param name="task">The faulted receiving task associated with this client</param>
        /// <returns>False if the fault cannot be recovered from</returns>
        public virtual bool ReceiveFault(Task task)
        {
            if (task.Exception != null)
            {
                Console.WriteLine(
                    "Receive task faulted, status: {0}, exception: {1}",
                    task.Status.ToString(),
                    task.Exception.ToString());
            }

            return false;
        }

        /// <summary>
        /// Sends a packet to the game client after encrypting bytes. This may be called
        /// as-is, or overridden to provide channel functionality and thread-safety around
        /// the accepted client socket. By default, this method locks around encryption
        /// and sending data. 
        /// </summary>
        /// <param name="packet">Bytes to be encrypted and sent to the client</param>
        public virtual void Send(byte[] packet)
        {
            var encrypted = new byte[packet.Length];
            BitConverter.TryWriteBytes(packet, (ushort)packet.Length);
            lock (SendLock)
            {
                try 
                {
                    this.Cipher.Encrypt(packet, encrypted);
                    this.Socket.Send(encrypted, SocketFlags.None);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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
        public virtual void Send(IPacket packet)
        {
            this.Send(packet.Encode());
        }

        /// <summary>
        /// Returns the remote IP address of the connected client.
        /// </summary>
        public string IPAddress => 
            (this.Socket.RemoteEndPoint as IPEndPoint).Address.MapToIPv4().ToString();
    }    
}
