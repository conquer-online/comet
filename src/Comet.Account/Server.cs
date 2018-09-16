namespace Comet.Account
{
    using System;
    using System.Net.Sockets;
    using Comet.Account.States;
    using Comet.Network.Security;
    using Comet.Network.Sockets;

    /// <summary>
    /// Server inherits from a base server listener to provide the account server with
    /// listening functionality and event handling. This class defines how the server 
    /// listener and invoked events are customized for the account server.
    /// </summary>
    internal sealed class Server : TcpServerListener
    {
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
    }
}
