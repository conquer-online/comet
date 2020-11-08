namespace Comet.Network.Packets
{
    /// <summary>
    /// Packet types for the Conquer Online game client across all server projects. 
    /// Identifies packets by an unsigned short from offset 2 of every packet sent to
    /// the server.
    /// </summary>
    public enum PacketType : ushort
    {
        MsgRegister = 1001,
        MsgTalk = 1004,
        MsgUserInfo = 1006,
        MsgItemInfo = 1008,
        MsgItem,
        MsgTick = 1012,
        MsgName = 1015,
        MsgWeather,
        MsgFriend = 1019,
        MsgInteract = 1022,
        MsgTeam,
        MsgAllot,
        MsgWeaponSkill,
        MsgTeamMember,
        MsgGemEmbed,
        MsgFuse,
        MsgTeamAward,
        MsgEnemyList = 1041,
        MsgMonsterTransform,
        MsgTeamRoll,
        MsgLoadMap,
        MsgTrade = 1056,
        MsgConnect = 1052,
        MsgConnectEx = 1055,
        MsgEncryptCode = 1059,
        MsgAccount = 1086,
        MsgMapItem = 1101,
        MsgPackage,
        MsgMagicInfo,
        MsgFlushExp,
        MsgMagicEffect,
        MsgSyndicateAttributeInfo,
        MsgSyndicate,
        MsgItemInfoEx,
        MsgNpcInfoEx,
        MsgMapInfo,
        MsgMessageBoard,
        MsgDice = 1113,
        MsgSyncAction,
        MsgNpcInfo = 2030,
        MsgNpc,
        MsgTaskDialog,
        MsgDataArray = 2036,
        MsgWalk = 10005,
        MsgAction = 10010,
        MsgPlayer = 10014,
        MsgUserAttrib = 10017
    }
}
