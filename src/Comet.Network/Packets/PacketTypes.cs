namespace Comet.Network.Packets
{
    /// <summary>
    /// Packet types for the Conquer Online game client across all server projects. 
    /// Identifies packets by an unsigned short from offset 2 of every packet sent to
    /// the server.
    /// </summary>
    public enum PacketType : ushort
    {
        MsgRegister  = 1001,
        MsgTalk      = 1004,
        MsgUserInfo  = 1006,
        MsgItem      = 1009,
        MsgAction    = 1010,
        MsgAccount   = 1051,
        MsgConnect   = 1052,
        MsgConnectEx = 1055
    }
}
