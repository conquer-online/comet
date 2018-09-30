namespace Comet.Network.RPC
{
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using ByteEncodings;
    using StreamJsonRpc;

    /// <summary>
    /// Creates a TCP client to connect to an RPC server listener. RPC is implemented as
    /// JSON-RPC over TCP (a web socket / network stream). Once the client connects, it can
    /// remain connected for the life of the TCP connection or until the server closes.
    /// The client may be encrypted for wire security, but deally should never be exposed 
    /// to the naked internet (use security groups or virtual networks if splitting between
    /// two VMs).
    /// </summary>
    public abstract class RpcClient
    {
        // Fields and Properties
        protected TcpClient BaseClient;
        private string Key, IV;
        private object Target;

        /// <summary>
        /// Instantiates a new instance of <see cref="RpcClient"/> using an optional AES key
        /// and IV for JSON-RPC stream security. 
        /// </summary>
        /// <param name="target">Target class for defining the RPC interface</param>
        /// <param name="key">AES key as a hexadecimal string</param>
        /// <param name="iv">AES IV as a hexadecimal string</param>
        public RpcClient(object target, string key = "", string iv = "")
        {
            this.Target = target;
            this.Key = key;
            this.IV = iv;
        }

        /// <summary>
        /// Connects the client to a remote TCP host using the specified host name and port
        /// number. Once the client has connected, the stream will be wrapped in a JSON-RPC
        /// protocol wrapper with AES security.
        /// </summary>
        /// <param name="address">IP address of the RPC server</param>
        /// <param name="port">Port the RPC server is listening on</param>
        /// <returns>Returns a new task for connecting and fault tolerance.</returns>
        public async Task Connect(string address, int port)
        {
            this.BaseClient = new TcpClient();
            await this.BaseClient.ConnectAsync(address, port);
            using (var stream = new NetworkStream(this.BaseClient.Client, true))
            {
                // Initialize AES stream security
                Stream input = stream; 
                Stream output = stream; 
                if (!string.IsNullOrEmpty(this.Key) && !string.IsNullOrEmpty(this.IV))
                {
                    var cipher = new AesCryptoServiceProvider();
                    ByteEncoding.Hex.ToBytes(this.Key, cipher.Key, true);
                    ByteEncoding.Hex.ToBytes(this.IV, cipher.IV, true);
                    var decrypt = cipher.CreateDecryptor(cipher.Key, cipher.IV);
                    var encrypt = cipher.CreateEncryptor(cipher.Key, cipher.IV);
                    input = new CryptoStream(stream, decrypt, CryptoStreamMode.Read);
                    output = new CryptoStream(stream, encrypt, CryptoStreamMode.Write);
                }

                // Attach JSON-RPC wrapper
                var rpc = JsonRpc.Attach(output, input, this.Target);
                await rpc.Completion;
            }
        }
    }
}
