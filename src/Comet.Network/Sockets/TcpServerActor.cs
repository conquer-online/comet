namespace Comet.Network.Sockets
{
    using System;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    /// <summary>
    /// Actors are assigned to accepted client sockets to give connected clients a state
    /// across socket operations. This allows the server to handle multiple receive writes
    /// across single processing reads, and keep a buffer alive for faster operations.
    /// </summary>
    public class TcpServerActor
    {
        // Fields and Properties
        public readonly Memory<byte> Buffer;
        public readonly Socket Socket;

        /// <summary>
        /// Instantiates a new instance of <see cref="TcpServerActor"/> using an accepted
        /// client socket and preallocated buffer from the server listener.
        /// </summary>
        /// <param name="socket">Accepted client socket</param>
        /// <param name="buffer">Preallocated buffer for socket receive operations</param>
        public TcpServerActor(Socket socket, Memory<byte> buffer)
        {
            this.Buffer = buffer;
            this.Socket = socket;
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
    }    
}
