namespace Comet.Account.States
{
    using System;
    using System.Net.Sockets;
    using Comet.Network.Security;
    using Comet.Network.Sockets;

    /// <summary>
    /// Client encapsules the accepted client socket's actor and account server state.
    /// The class should be initialized by the server's Accepted method and returned
    /// to be passed along to the Receive loop and kept alive.
    /// </summary>
    internal sealed class Client : TcpServerActor
    {
        /// <summary>
        /// Instantiates a new instance of <see cref="Client"/> using the Accepted event's
        /// resulting socket and preallocated buffer. Initializes all account server
        /// states, such as the cipher used to decrypt and encrypt data.
        /// </summary>
        /// <param name="socket">Accepted remote client socket</param>
        /// <param name="buffer">Preallocated buffer from the server listener</param>
        public Client(Socket socket, Memory<byte> buffer) 
            : base(socket, buffer, new TQCipher())
        {
            
        }
    }
}
