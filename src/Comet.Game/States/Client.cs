namespace Comet.Game.States
{
    using System;
    using System.Net.Sockets;
    using Comet.Network.Security;
    using Comet.Network.Sockets;

    /// <summary>
    /// Client encapsules the accepted client socket's actor and game server state.
    /// The class should be initialized by the server's Accepted method and returned
    /// to be passed along to the Receive loop and kept alive. Contains all world
    /// interactions with the player.
    /// </summary>
    public sealed class Client : TcpServerActor
    {
        // Fields and Properties 
        public Character Character = null;
        public Creation Creation = null;
        public DiffieHellman DiffieHellman = null;

        // Client unique identifier
        public uint ID => Character?.CharacterID ?? 0;

        /// <summary>
        /// Instantiates a new instance of <see cref="Client"/> using the Accepted event's
        /// resulting socket and preallocated buffer. Initializes all account server
        /// states, such as the cipher used to decrypt and encrypt data.
        /// </summary>
        /// <param name="socket">Accepted remote client socket</param>
        /// <param name="buffer">Preallocated buffer from the server listener</param>
        /// <param name="partition">Packet processing partition</param>
        public Client(Socket socket, Memory<byte> buffer, uint partition) 
            : base(socket, buffer, new BlowfishCipher(BlowfishCipher.Default), partition, "TQServer")
        {
            this.DiffieHellman = new DiffieHellman();
        }
    }
}
