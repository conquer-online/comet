namespace Comet.Network.Security.Tests
{
    using System.Collections.Generic;
    using Comet.Network.Packets;
    using Org.BouncyCastle.Utilities.Encoders;
    using Xunit;

    /// <summary>
    /// Packets are written using a base byte stream, overloaded to support TQ's binary 
    /// byte ordering rules and protocol.
    /// </summary>
    public class PacketWriterTests
    {
        const string ExpectedOutput = 
            "0000e90301020003000000040000000000000004746573746" +
            "36f6d657400000000000000020568656c6c6f05776f726c64";

        [Fact]
        public void PacketWriteTest()
        {
            PacketWriter packet = new PacketWriter();
            packet.Write((ushort)PacketType.MsgRegister);
            packet.Write((byte)1);
            packet.Write((ushort)2);
            packet.Write((uint)3);
            packet.Write((ulong)4);
            packet.Write("test");
            packet.Write("comet", 12);
            packet.Write(new List<string> { "hello", "world" });
            Assert.Equal(ExpectedOutput, Hex.ToHexString(packet.ToArray()));
        }
    }
}
