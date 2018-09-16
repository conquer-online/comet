namespace Comet.Network.Packets
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Dumps packet bytes in a human-readable format. Primarily used to debug server 
    /// errors and missing packet structures, or to reverse engineer unknown packet
    /// structures.
    /// </summary>
    public static class PacketDump
    {
        /// <summary>
        /// Converts packet bytes to a hexadecimal string. The format of the hex dump
        /// matches the output of hexdump -C from Linux command line.
        /// </summary>
        /// <param name="data">Packet data to be formatted</param>
        /// <returns>Returns the hexadecimal string created by Hex.</returns>
        public static string Hex(ReadOnlySpan<byte> data)
        {
            var text = new StringBuilder();
            for (int l = 0; l < data.Length; l += 16) 
            {
                // Write the address and body
                text.AppendFormat("{0:X4}:", l);
                for (int i = l; i < l + 16; i++) 
                {
                    text.Append(i % 8 == 0 ? "  " : " ");
                    text.Append(i >= data.Length ? "  " : 
                        string.Format("{0:X2}", data[i]));
                }

                // Write the ASCII conversion
                int v = l + 16 >= data.Length ? data.Length : l + 16;
                text.Append("  | ");
                for (int i = l; i < v; i++) 
                    text.Append(data[i] < 32 || data[i] > 126 ? '.' : (char)data[i]);
                text.Append(" |\n");
            }

            return text.ToString();
        }
    }
}
