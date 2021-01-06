namespace Comet.Network.Packets
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Reader that implements methods for reading bytes from a binary stream reader,
    /// used to help decode packet structures using TQ Digital's byte ordering rules.
    /// String processing has been overloaded for supporting TQ's byte-length prefixed
    /// strings and fixed strings.
    /// </summary>
    public sealed class PacketReader : BinaryReader, IDisposable
    {
        /// <summary>
        /// Instantiates a new instance of <see cref="PacketReader"/> using a supplied
        /// array of packet bytes. Creates a new binary reader for the derived class
        /// to read from.
        /// </summary>
        /// <param name="bytes">Packet bytes to be read in</param>
        public PacketReader(byte[] bytes) : base(new MemoryStream(bytes))
        {
        }

        /// <summary>
        /// Reads a string from the current stream. The string is prefixed with the byte
        /// length and encoded as an ASCII string. <see cref="EndOfStreamException"/> is
        /// thrown if the full string cannot be read from the binary reader.
        /// </summary>
        /// <returns>Returns the resulting string from the read.</returns>
        public override string ReadString()
        {
            return base.ReadString().TrimEnd('\0');
        }

        /// <summary>
        /// Reads a string from the current stream. The string is fixed with a known
        /// string length before reading from the stream and encoded as an ASCII string.
        /// <see cref="EndOfStreamException"/> is thrown if the full string cannot be 
        /// read from the binary reader.
        /// </summary>
        /// <param name="fixedLength">Length of the string to be read</param>
        /// <returns>Returns the resulting string from the read.</returns>
        public string ReadString(int fixedLength)
        {
            return Encoding.ASCII.GetString(base.ReadBytes(fixedLength)).TrimEnd('\0');
        }

        /// <summary>
        /// Reads a list of strings from the current stream. The string list is prefixed
        /// with the byte amount of strings in the list. Then, each string in the list is
        /// prefixed with the length of that string and encoded as an ASCII string. 
        /// <see cref="EndOfStreamException"/> is thrown if the full string cannot be read
        /// from the binary reader.
        /// </summary>
        /// <returns>Returns the resulting list of strings from the read.</returns>
        public List<string> ReadStrings()
        {
            var strings = new List<string>();
            var amount = base.ReadByte();
            for (int i = 0; i < amount; i++)
                strings.Add(this.ReadString());
            return strings;
        }

        #region IDisposable Support
        private bool DisposedValue = false; // To detect redundant calls

        /// <summary>
        /// Called from the Dispose method to dispose of class resources once and only
        /// once using the Disposable design pattern. Calls into the base dispose method
        /// after disposing of class resources first.
        /// </summary>
        /// <param name="disposing">True if clearing unmanaged and managed resources</param>
        private new void Dispose(bool disposing)
        {
            if (!this.DisposedValue)
            {
                if (disposing)
                {
                    base.BaseStream.Close();
                    base.BaseStream.Dispose();
                }

                base.Dispose(disposing);
                this.DisposedValue = true;
            }
        }

        /// <summary>
        /// Called to dispose the class. 
        /// </summary>
        public new void Dispose()
        {
            this.Dispose(true);
        }
        #endregion
    }
}
