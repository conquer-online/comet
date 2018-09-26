namespace Comet.Network.Packets
{
    using System.IO;

    /// <summary>
    /// Writer that implements methods for writing bytes to a binary stream writer, used 
    /// to help encode packet structures using TQ Digital's byte ordering rules. Packets
    /// sent using the <see cref="TcpServerActor"/>'s send method do not need to be
    /// written with a prefixed length. The send method will include this with the final
    /// result of the packet writer.
    /// </summary>
    public sealed class PacketWriter : BinaryWriter
    {
        /// <summary>
        /// Instantiates a new instance of <see cref="PacketWriter"/> and writes the 
        /// first unsigned short to the stream as a placeholder for the packet length.
        /// </summary>
        public PacketWriter()
        {
            this.Write((ushort)0);
        }
    }
}
