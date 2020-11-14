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
        MsgWalk,
        MsgUserInfo,
        MsgItemInfo = 1008,
        MsgItem,
        MsgAction,
        MsgTick = 1012,
        MsgPlayer = 1014,
        MsgName,
        MsgWeather,
        MsgUserAttrib,
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
        MsgAccount = 1051,
        MsgConnect,
        MsgConnectEx = 1055,
        MsgTrade,
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
        MsgDataArray = 2036
    }
}
