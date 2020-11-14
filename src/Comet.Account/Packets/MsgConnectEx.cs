namespace Comet.Account.Packets
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Comet.Account.States;
    using Comet.Network.Packets;

    /// <remarks>Packet Type 1055</remarks>
    /// <summary>
    /// Message containing an authentication response from the server to the client. Can
    /// either accept a client with realm connection details and an access token, or 
    /// reject a client with a reason on why the login attempt was rejected.
    /// </summary>
    public sealed class MsgConnectEx : MsgBase<Client>
    {
        // Constants and static properties
        private static Encoding GB2312;
        private static Dictionary<RejectionCode, string> RejectionMessages;

        // Packet Properties
        public ulong Token { get; set; }
        public RejectionCode Code { get; set; }
        public string IPAddress { get; set; }
        public uint Port { get; set; }

        /// <summary>
        /// Initialize the dictionary mapping rejection codes to Chinese rejection
        /// messages on first instantiation of the class. Since GB2312 isn't supported
        /// in dotnet core, register the external encoding provider.
        /// </summary>
        static MsgConnectEx()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            MsgConnectEx.GB2312 = Encoding.GetEncoding("GB2312");

            MsgConnectEx.RejectionMessages = new Dictionary<RejectionCode, string>();
            MsgConnectEx.RejectionMessages.Add(RejectionCode.InvalidPassword, "帐号名或口令错");
            MsgConnectEx.RejectionMessages.Add(RejectionCode.ServerDown, "服务器未启动");
            MsgConnectEx.RejectionMessages.Add(RejectionCode.ServerBusy, "请稍后重新登录");
            MsgConnectEx.RejectionMessages.Add(RejectionCode.AccountBanned, "该帐号被封号");
            MsgConnectEx.RejectionMessages.Add(RejectionCode.UnknownError, "数据库错误");
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="MsgConnectEx"/> for rejecting a
        /// client connection using a rejection code. The rejection code spawns an error
        /// dialog in the client with a respective error message.
        /// </summary>
        /// <param name="rejectionCode">Rejection code / error message ID</param>
        public MsgConnectEx(RejectionCode rejectionCode)
        {
            this.Code = rejectionCode;
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="MsgConnectEx"/> for accepting a
        /// client connection. Provides the client with redirect information for the game
        /// server of their choice, accompanied by an access token for that server.
        /// </summary>
        /// <param name="ipAddress">IP address of the game server</param>
        /// <param name="port">Port the game server is listening on</param>
        /// <param name="token">Access token for the game server</param>
        public MsgConnectEx(string ipAddress, uint port, ulong token)
        {
            this.Token = token;
            this.IPAddress = ipAddress;
            this.Port = port;
        }

        /// <summary>
        /// Encodes the packet structure defined by this message class into a byte packet
        /// that can be sent to the client. Invoked automatically by the client's send 
        /// method. Encodes using byte ordering rules interoperable with the game client.
        /// </summary>
        /// <returns>Returns a byte packet of the encoded packet.</returns>
        public override byte[] Encode()
        {
            var writer = new PacketWriter();
            writer.Write((ushort)PacketType.MsgConnectEx);
            if (this.Code != RejectionCode.Clear)
            {
                // Failed login
                writer.Write((uint)0);
                writer.Write((uint)this.Code);
                writer.Write(
                    MsgConnectEx.RejectionMessages[this.Code], 
                    MsgConnectEx.GB2312, 
                    16);
            }
            else
            {
                // Successful login
                writer.Write(this.Token);
                writer.Write(this.IPAddress, 16);
                writer.Write(this.Port);
            }
            
            return writer.ToArray();
        }

        /// <summary>
        /// Rejection codes are sent to the client in offset 8 of this packet when the
        /// client has failed authentication with the account server. These codes define
        /// which error message will be displayed in the client.
        /// </summary>
        public enum RejectionCode : uint
        {
            Clear = 0,
            InvalidPassword = 1,
            ServerDown = 10,
            ServerBusy = 11,
            AccountBanned = 12,
            UnknownError = 999
        }

        /// <summary>
        /// In older patches of Conquer Online, rejection codes had to be accompanied by
        /// a Chinese rejection message in GB2312 encoding. The Chinese encoded message is
        /// used to map the hardcoded Chinese string to an English string from StrRes.ini.
        /// </summary>
        public static class RejectionMessage
        {
            public const string InvalidPassword = "帐号名或口令错";
            public const string ServerDown = "服务器未启动";
            public const string ServerBusy = "请稍后重新登录";
            public const string AccountBanned = "该帐号被封号";
            public const string UnknownError = "数据库错误";
        }
    }
}
