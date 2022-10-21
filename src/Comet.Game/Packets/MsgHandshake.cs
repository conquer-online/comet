namespace Comet.Game.Packets
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Comet.Game.States;
    using Comet.Network.Packets;
    using Comet.Network.Security;
    using Org.BouncyCastle.Utilities.Encoders;

    /// <summary>
    /// Message containing keys necessary for conducting the Diffie-Hellman key exchange.
    /// The initial message to the client is sent on connect, and contains initial seeds
    /// for Blowfish. The response message from the client then contains the shared key. 
    /// </summary>
    public sealed class MsgHandshake : MsgBase<Client>
    {
        // Packet Properties
        public byte[] DecryptionIV { get; private set; }
        public byte[] EncryptionIV { get; private set; }
        public string PrimeRoot { get; private set; }
        public string Generator { get; private set; }
        public string ServerKey { get; private set; }
        public string ClientKey { get; private set; }
        private byte[] Padding { get; set; }

        /// <summary>
        /// Instantiates a new instance of <see cref="MsgHandshake"/>. This constructor
        /// is called to accept the client response.
        /// </summary>
        public MsgHandshake()
        {
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="MsgHandshake"/>. This constructor
        /// is called to construct the initial request to the client.
        /// </summary>
        /// <param name="dh">Diffie-Hellman key exchange instance for the actor</param>
        /// <param name="encryptionIV">Initial seed for Blowfish's encryption IV</param>
        /// <param name="decryptionIV">Initial seed for Blowfish's decryption IV</param>
        public MsgHandshake(DiffieHellman dh, byte[] encryptionIV, byte[] decryptionIV)
        {
            this.PrimeRoot = Hex.ToHexString(dh.PrimeRoot.ToByteArrayUnsigned()).ToUpper();
            this.Generator = Hex.ToHexString(dh.Generator.ToByteArrayUnsigned()).ToUpper();
            this.ServerKey = Hex.ToHexString(dh.PublicKey.ToByteArrayUnsigned()).ToUpper();
            this.EncryptionIV = (byte[])encryptionIV.Clone();
            this.DecryptionIV = (byte[])decryptionIV.Clone();
        }

        /// <summary>Randomizes padding for the message.</summary>
        public async Task RandomizeAsync()
        {
            this.Padding = new byte[23];
            await Kernel.NextBytesAsync(this.Padding);
        }

        /// <summary>
        /// Decodes a byte packet into the packet structure defined by this message class. 
        /// Should be invoked to structure data from the client for processing. Decoding
        /// follows TQ Digital's byte ordering rules for an all-binary protocol.
        /// </summary>
        /// <param name="bytes">Bytes from the packet processor or client socket</param>
        public override void Decode(byte[] bytes)
        {
            var reader = new PacketReader(bytes);
            reader.BaseStream.Seek(7, SeekOrigin.Begin);
            this.Length = (ushort)reader.ReadUInt32();
            reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Current);
            this.ClientKey = Encoding.ASCII.GetString(reader.ReadBytes(reader.ReadInt32()));
        }

        /// <summary>
        /// Encodes the packet structure defined by this message class into a byte packet
        /// that can be sent to the client. Invoked automatically by the client's send 
        /// method. Encodes using byte ordering rules interoperable with the game client.
        /// </summary>
        /// <returns>Returns a byte packet of the encoded packet.</returns>
        public override byte[] Encode()
        {
            var writer = new PacketWriter();
            var messageLength = 36 + this.Padding.Length + this.EncryptionIV.Length 
                + this.DecryptionIV.Length + this.PrimeRoot.Length + this.Generator.Length
                + this.ServerKey.Length;

            // The packet writer class reserves 2 bytes for the send method to fill in for
            // the packet length. This message is an outlier to this pattern; however, 
            // leaving the reserved bytes does not affect the body of the message, so it
            // can be left in.

            writer.Write(this.Padding.AsSpan(0, 9));
            writer.Write(messageLength - 11);
            writer.Write(this.Padding.Length - 11);
            writer.Write(this.Padding.AsSpan(9, this.Padding.Length - 11));
            writer.Write(this.EncryptionIV.Length);
            writer.Write(this.EncryptionIV);
            writer.Write(this.DecryptionIV.Length);
            writer.Write(this.DecryptionIV);
            writer.Write(this.PrimeRoot.Length);
            writer.Write(this.PrimeRoot, this.PrimeRoot.Length);
            writer.Write(this.Generator.Length);
            writer.Write(this.Generator, this.Generator.Length);
            writer.Write(this.ServerKey.Length);
            writer.Write(this.ServerKey, this.ServerKey.Length);
            return writer.ToArray();
        }
    }
}