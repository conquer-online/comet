namespace Comet.Network.Packets
{
    /// <summary>
    /// Packet types for the Conquer Online game client across all server projects. 
    /// Identifies packets by an unsigned short from offset 2 of every packet sent to
    /// the server.
    /// </summary>
    public enum PacketType : ushort
    {
        MsgAccount = 1051,
        MsgConnectEx = 1055
    }
}
