namespace Comet.Network.Sockets
{
    using System;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Comet.Network.Packets;

    /// <summary>
    /// TcpServerEvents provides default socket events handlers for server event handling,
    /// invoked by the server listener when processing common socket operations for server
    /// actors. This allows a server listener to define unique socket events across 
    /// multiple projects via inheritance.
    /// </summary>
    /// <typeparam name="TActor">Type of actor passed by the parent project</typeparam>
    public abstract class TcpServerEvents<TActor> 
        where TActor : TcpServerActor
    {
        /// <summary>
        /// Invoked by the server listener's Accepting method to create a new server actor
        /// around the accepted client socket. Gives the server an opportunity to initialize
        /// any processing mechanisms or authentication routines for the client connection.
        /// </summary>
        /// <param name="socket">Accepted client socket from the server socket</param>
        /// <param name="buffer">Preallocated buffer from the server listener</param>
        /// <returns>A new instance of a TActor around the client socket</returns>
        protected abstract Task<TActor> AcceptedAsync(Socket socket, Memory<byte> buffer);

        /// <summary>
        /// Invoked by the server listener's Exchanging method to process the client 
        /// response from the Diffie-Hellman Key Exchange. At this point, the raw buffer 
        /// from the client has been decrypted and is ready for direct processing.
        /// </summary>
        /// <param name="actor">Server actor that represents the remote client</param>
        /// <param name="buffer">Packet buffer to be processed</param>
        /// <returns>True if the exchange was successful.</returns>
        protected virtual bool Exchanged(TActor actor, ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invoked by the server listener's Receiving method to process a completed packet
        /// from the actor's socket pipe. At this point, the packet has been assembled and
        /// split off from the rest of the buffer. Default behavior, if not overridden, is 
        /// to print the packet bytes to the console screen as a hex dump.
        /// </summary>
        /// <param name="actor">Server actor that represents the remote client</param>
        /// <param name="packet">Packet bytes to be processed</param>
        protected virtual void Received(TActor actor, ReadOnlySpan<byte> packet)
        {
            Console.WriteLine("Received {0} bytes", packet.Length);
            Console.WriteLine(PacketDump.Hex(packet));
        }

        /// <summary>
        /// Invoked by the server listener's Disconnecting method to dispose of the actor's
        /// resources. Gives the server an opportunity to cleanup references to the actor
        /// from other actors and server collections.
        /// </summary>
        /// <param name="actor">Server actor that represents the remote client</param>
        protected virtual void Disconnected(TActor actor) 
        {
        }
    }
}
