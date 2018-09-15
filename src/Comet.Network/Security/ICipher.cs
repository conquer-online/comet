namespace Comet.Network.Security
{
    using System;

    /// <summary>
    /// Defines generalized methods for ciphers used by 
    /// <see cref="Comet.Network.Sockets.TcpServerActor"/> and 
    /// <see cref="Comet.Network.Sockets.TcpServerListener"/> for encrypting and decrypting
    /// data to and from the game client. Can be used to switch between ciphers easily for
    /// seperate states of the game client connection.
    /// </summary>
    public interface ICipher
    {
        /// <summary>Generates keys using key derivation variables.</summary>
        /// <param name="seeds">Initialized seeds for generating keys</param>
        void GenerateKeys(object[] seeds);

        /// <summary>Decrypts data from the client</summary>
        /// <param name="src">Source span that requires decrypting</param>
        /// <param name="dst">Destination span to contain the decrypted result</param>
        void Decrypt(Span<byte> src, Span<byte> dst);
        
        /// <summary>Encrypts data to send to the client</summary>
        /// <param name="src">Source span that requires encrypting</param>
        /// <param name="dst">Destination span to contain the encrypted result</param>
        void Encrypt(Span<byte> src, Span<byte> dst);
    }
}