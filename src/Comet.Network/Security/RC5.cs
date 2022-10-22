namespace Comet.Network.Security
{
    using System;
    using Comet.Core.Mathematics;

    /// <summary>
    /// Rivest Cipher 5 is implemented for interoperability with the Conquer Online game 
    /// client's login procedure. Passwords are encrypted in RC5 by the client, and decrypted
    /// on the server to be hashed and compared to the database saved password hash. In
    /// newer clients, this was replaced with SRP-6A (a hash based exchange protocol).
    /// </summary>
    public sealed class RC5 : ICipher
    {
        // Constants and static properties
        private const int WordSize = 16;
        private const int Rounds = 12;
        private const int KeySize = RC5.WordSize / 4;
        private const int SubSize = 2 * (RC5.Rounds + 1);

        // Local fields and properties
        private readonly uint[] Key, Sub;

        /// <summary>
        /// Initializes static variables for <see cref="RC5"/> to be interoperable with
        /// the Conquer Online game client. In later versions of the client, a random
        /// buffer is used to seed the cipher. This random buffer is sent to the client
        /// to establish a shared initial key.
        /// </summary>
        public RC5()
        {
            this.Key = new uint[RC5.KeySize];
            this.Sub = new uint[RC5.SubSize];
            this.GenerateKeys(new object[] { new byte[] { 
                0x3C, 0xDC, 0xFE, 0xE8, 0xC4, 0x54, 0xD6, 0x7E, 
                0x16, 0xA6, 0xF8, 0x1A, 0xE8, 0xD0, 0x38, 0xBE  
            } });
        }

        /// <summary>
        /// Generates keys and the subkey words for RC5 using a shared seed, whether
        /// that seed is shared statically or shared using a method of transport. Though
        /// only one seed is expected to generate keys, multiple may be used. Seed must be
        /// divisible by the selected cipher word size (16 bytes in this implementation).
        /// </summary>
        /// <param name="seeds">An array of seeds used to generate keys</param>
        public void GenerateKeys(object[] seeds)
        {
            // Initialize key expansion
            var seedBuffer = seeds[0] as byte[];
            var seedLength = seedBuffer.Length / RC5.WordSize * RC5.WordSize;
            for (int i = 0; i < RC5.KeySize; i++)
                this.Key[i] = BitConverter.ToUInt32(seedBuffer, i * 4);

            // Generate subkey words
            this.Sub[0] = 0xB7E15163;
            for (int i = 1; i < RC5.SubSize; i++)
                this.Sub[i] = this.Sub[i - 1] - 0x61C88647;

            // Generate key vector
            for (uint x = 0, i = 0, j = 0, a = 0, b = 0; x < 3 * RC5.SubSize; x++)
            {
                a = this.Sub[i] = (this.Sub[i] + a + b).RotateLeft(3);
                b = this.Key[j] = (this.Key[j] + a + b).RotateLeft((int)(a + b));
                i = (i + 1) % RC5.SubSize;
                j = (j + 1) % RC5.KeySize;
            }
        }

        /// <summary>
        /// Decrypts bytes from the client. If the buffer passed is not a multiple of
        /// the word size divisor in bytes, then pads the buffer with zeroes. The source 
        /// and destination may not be the same slice.
        /// </summary>
        /// <param name="src">Source span that requires decrypting</param>
        /// <param name="dst">Destination span to contain the decrypted result</param>
        public void Decrypt(Span<byte> src, Span<byte> dst)
        {
            // Pad the buffer
            var length = src.Length / 8;
            if (src.Length % 8 > 0) length = length + 1;
            src.CopyTo(dst);

            // Decrypt the buffer
            for (int word = 0; word < length; word++)
            {
                uint a = BitConverter.ToUInt32(dst[(8 * word)..]);
                uint b = BitConverter.ToUInt32(dst[(8 * word + 4)..]);
                for (int round = RC5.Rounds; round > 0; round--)
                {
                    b = (b - this.Sub[2 * round + 1]).RotateRight((int)a) ^ a;
                    a = (a - this.Sub[2 * round]).RotateRight((int)b) ^ b;
                }

                BitConverter.GetBytes(a - this.Sub[0]).CopyTo(dst[(8 * word)..]);
                BitConverter.GetBytes(b - this.Sub[1]).CopyTo(dst[(8 * word + 4)..]);
            }
        }

        /// <summary>
        /// Encrypts bytes from the server. If the buffer passed is not a multiple of the
        /// word size divisor in bytes, then pads the buffer with zeroes. The source 
        /// and destination may not be the same slice.
        /// </summary>
        /// <param name="src">Source span that requires encrypting</param>
        /// <param name="dst">Destination span to contain the encrypted result</param>
        public void Encrypt(Span<byte> src, Span<byte> dst)
        {
            // Pad the buffer
            var length = src.Length / 8;
            if (src.Length % 8 > 0) length = length + 1;
            dst = new byte[length * 8];
            src.CopyTo(dst[..src.Length]);

            // Decrypt the buffer
            for (int word = 0; word < length; word++)
            {
                uint a = BitConverter.ToUInt32(dst[(8 * word)..]) + this.Sub[0];
                uint b = BitConverter.ToUInt32(dst[(8 * word + 4)..]) + this.Sub[1];
                for (int round = 1; round <= RC5.Rounds; round++)
                {
                    a = (a ^ b).RotateLeft((int)b) + this.Sub[2 * round];
                    b = (b ^ a).RotateLeft((int)a) + this.Sub[2 * round + 1];
                }

                BitConverter.GetBytes(a).CopyTo(dst[(8 * word)..]);
                BitConverter.GetBytes(b).CopyTo(dst[(8 * word + 4)..]);
            }
        }
    }
}
