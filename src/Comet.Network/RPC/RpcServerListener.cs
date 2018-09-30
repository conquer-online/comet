namespace Comet.Network.RPC
{
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using ByteEncodings;
    using StreamJsonRpc;

    /// <summary>
    /// Creates a TCP listener to listen for multiple remote procedure call requests on.
    /// RPC is implemented as JSON-RPC over TCP (the web socket / network stream). Once
    /// connected, clients will continue operating and sending requests for the life of
    /// the TCP connection or until the client is closed. The RPC network stream may be
    /// encrypted for wire security, but ideally should never be exposed to the naked
    /// internet (use security groups or virtual networks if splitting between two VMs).
    /// </summary>
    public class RpcServerListener
    {
        // Fields and Properties
        protected TcpListener BaseListener;
        protected CancellationTokenSource ShutdownToken;
        private byte[] Key, IV;
        private object Target;

        /// <summary>
        /// Instantiates a new instance of <see cref="RpcServerListener"/> using an
        /// optional AES key and IV for JSON-RPC stream security. 
        /// </summary>
        /// <param name="target">Target class for defining the RPC interface</param>
        /// <param name="key">AES key as a hexadecimal string</param>
        /// <param name="iv">AES IV as a hexadecimal string</param>
        public RpcServerListener(object target, string key = "", string iv = "")
        {
            this.Target = target;
            this.Key = new byte[key.Length / 2];
            this.IV = new byte[iv.Length / 2];
            ByteEncoding.Hex.ToBytes(key, this.Key);
            ByteEncoding.Hex.ToBytes(iv, this.IV);
        }

        /// <summary>
        /// Binds the TCP listener to a port and network interface. Specify "0.0.0.0" 
        /// as the endpoint address to bind to all interfaces on the host machine. Starts
        /// the server listener and accepts new connections in a new task.
        /// </summary>
        /// <param name="port">Port number the server will bind to</param>
        /// <param name="address">Interface IPv4 address the server will bind to</param>
        /// <returns>Returns a new task for accepting new connections.</returns>
        public Task Start(int port, string address = "0.0.0.0")
        {
            this.ShutdownToken = new CancellationTokenSource();
            this.BaseListener = new TcpListener(IPAddress.Parse(address), port);
            this.BaseListener.Start();
            return this.Accepting();
        }

        /// <summary>
        /// Accepting accepts client connections asynchronously as a new task. As a client
        /// connection is accepted, it will be associated with a new JSON-RPC wrapper. The 
        /// server will start processing requests from the client immediately after.
        /// </summary>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task Accepting()
        {
            while (this.BaseListener.Server.IsBound && !this.ShutdownToken.IsCancellationRequested)
            {
                var socket = await this.BaseListener.AcceptSocketAsync();
                var task = Task.Run(() => this.Receiving(socket));
            }
        }

        /// <summary>
        /// Receives commands from the RPC client connection. The connected network stream
        /// will be converted to a cipherstream using AES, and then attached to the JSON-RPC
        /// protocol wrapper for the connection.
        /// </summary>
        /// <param name="socket">Accepted client socket</param>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task Receiving(Socket socket)
        {
            using (var stream = new NetworkStream(socket, true))
            {
                // Initialize AES stream security
                Stream input = stream; 
                Stream output = stream; 
                if (this.Key.Length > 0 && this.IV.Length > 0)
                {
                    var cipher = new AesCryptoServiceProvider();
                    cipher.Key = this.Key;
                    cipher.IV = this.IV;

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
