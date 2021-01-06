namespace Comet.Network.Security.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using Comet.Network.Packets;
    using Org.BouncyCastle.Utilities.Encoders;
    using Xunit;

    /// <summary>
    /// Packets are read using a base byte stream, overloaded to support TQ's binary 
    /// byte ordering rules and protocol.
    /// </summary>
    public class PacketReaderTests
    {
        const string InputBuffer = 
            "0000e90301020003000000040000000000000004746573746" +
            "36f6d657400000000000000020568656c6c6f05776f726c64";

        [Fact]
        public void PacketReadTest()
        {
            PacketReader packet = new PacketReader(Hex.Decode(InputBuffer));
            Assert.Equal((ushort)0, packet.ReadUInt16());
            Assert.Equal((ushort)PacketType.MsgRegister, packet.ReadUInt16());
            Assert.Equal((byte)1, packet.ReadByte());
            packet.BaseStream.Seek(-1, SeekOrigin.Current);
            Assert.True(packet.ReadBoolean());
            Assert.Equal((ushort)2, packet.ReadUInt16());
            Assert.Equal((uint)3, packet.ReadUInt32());
            Assert.Equal((ulong)4, packet.ReadUInt64());
            Assert.Equal("test", packet.ReadString());
            Assert.Equal("comet", packet.ReadString(12));
            Assert.Equal(new List<string>{ "hello", "world" }, packet.ReadStrings());
        }
    }
}
