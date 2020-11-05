namespace Comet.Network.Security
{
    using System;
    using System.Linq;

    /// <summary>
    /// A simple substitution cipher used for client passwords, which maps client key
    /// scan codes to ASCII characters using the account name as a key derivation 
    /// variable. The scan codes do not follow a tranditional US keyboard's scan code map.
    /// </summary>
    public sealed class ScanCodeCipher : ICipher
    {
        // Constants and static properties
        private const int SeedSize = 16;
        private static readonly byte[] Key;

        // Local fields and properties
        private byte[] Sub;

        /// <summary>
        /// Initializes static variables for <see cref="ScanCodeCipher"/>.
        /// </summary>
        static ScanCodeCipher()
        {
            ScanCodeCipher.Key = new byte[] {
                0x00, 0x00, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x30,
                0x2D, 0x00, 0x00, 0x00, 0x71, 0x77, 0x65, 0x72, 0x74, 0x79, 0x75, 0x69,
                0x6F, 0x70, 0x00, 0x00, 0x00, 0x00, 0x61, 0x73, 0x64, 0x66, 0x67, 0x68,
                0x6A, 0x6B, 0x6C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7A, 0x78, 0x63, 0x76,
                0x62, 0x6E, 0x6D, 0x00, 0x2E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x37,
                0x38, 0x39, 0x00, 0x34, 0x35, 0x36, 0x00, 0x31, 0x32, 0x33, 0x30, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x21, 0x40,
                0x23, 0x24, 0x25, 0x5E, 0x26, 0x2A, 0x28, 0x29, 0x5F, 0x00, 0x00, 0x00,
                0x51, 0x57, 0x45, 0x52, 0x54, 0x59, 0x55, 0x49, 0x4F, 0x50, 0x00, 0x00,
                0x00, 0x00, 0x41, 0x53, 0x44, 0x46, 0x47, 0x48, 0x4A, 0x4B, 0x4C, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x5A, 0x58, 0x43, 0x56, 0x42, 0x4E, 0x4D, 0x00,
                0x3E
            };
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="ScanCodeCipher"/> using the client
        /// input username as a key derivation variable for generating the substitution
        /// buffer. Uses the same seed generator as RC5 for client interoperability.
        /// </summary>
        /// <param name="username">The account name from client input</param>
        public ScanCodeCipher(string username)
        {
            this.Sub = new byte[0x200];

            int seed = username.Sum(x => (byte)x);
            byte[] seeds = new byte[ScanCodeCipher.SeedSize];
            for (int i = 0; i < seeds.Length; i++)
            {
                seed *= 0x343fd;
                seed += 0x269ec3;
                seeds[i] = (byte)((seed >> ScanCodeCipher.SeedSize) & 0x7fff);
            }

            this.GenerateKeys(new object[] { seeds });
        }

        /// <summary>
        /// Generates the substitution buffer for substituting scan codes in with plain
        /// text characters. Accepts a seed buffer generated using the client input 
        /// username.
        /// </summary>
        /// <param name="seeds">An array of seeds used to generate keys</param>
        public void GenerateKeys(object[] seeds)
        {
            // Initialize seed buffer
            var seedBuffer = seeds[0] as byte[];
            for (int i = 1; i < 0x100; i++) 
            {
                this.Sub[i * 2] = (byte)i;
                this.Sub[(i * 2) + 1] = (byte)(i ^ seedBuffer[i & 15]);
            }

            // Generate substitutions
            for (int k = 1; k < 0x100; k++)
            {
                for (int m = k + 1; m < 0x100; m++)
                {
                    int a = m * 2, b = k * 2;
                    if (this.Sub[b + 1] < this.Sub[a + 1])
                    {
                        this.Sub[b] = (byte)(this.Sub[b] ^ this.Sub[a]);
                        this.Sub[a] = (byte)(this.Sub[a] ^ this.Sub[b]);
                        this.Sub[b] = (byte)(this.Sub[b] ^ this.Sub[a]);
                        this.Sub[b + 1] = (byte)(this.Sub[b + 1] ^ this.Sub[a + 1]);
                        this.Sub[a + 1] = (byte)(this.Sub[a + 1] ^ this.Sub[b + 1]);
                        this.Sub[b + 1] = (byte)(this.Sub[b + 1] ^ this.Sub[a + 1]);
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts bytes by substituting scan codes using generated key indexes.
        /// </summary>
        /// <param name="src">Source span that requires decrypting</param>
        /// <param name="dst">Destination span to contain the decrypted result</param>
        public void Decrypt(Span<byte> src, Span<byte> dst)
        {
            for (int i = 0; i < src.Length; i++)
            {
                // Check for the end of the string
                if (src[i] == 0) 
                {
                    dst.Slice(i, dst.Length - i).Fill(0);
                    return;
                }

                // Substitute the byte
                byte position = (byte)(this.Sub[src[i] * 2]);
                dst[i] = ScanCodeCipher.Key[position % ScanCodeCipher.Key.Length];
            }
        }

        /// <summary>
        /// Encrypt is not implemented on the server.
        /// </summary>
        /// <param name="src">Source span that requires encrypting</param>
        /// <param name="dst">Destination span to contain the encrypted result</param>
        public void Encrypt(Span<byte> src, Span<byte> dst)
        {
            throw new NotImplementedException();
        }
    }
}