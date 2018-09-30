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
    public abstract class RpcServerListener
    {
        // Fields and Properties
        protected TcpListener BaseListener;
        protected CancellationTokenSource ShutdownToken;
        private string Key, IV;

        /// <summary>
        /// Instantiates a new instance of <see cref="RpcServerListener"/> using an
        /// optional AES key and IV for JSON-RPC stream security. 
        /// </summary>
        /// <param name="key">AES key as a hexadecimal string</param>
        /// <param name="iv">AES IV as a hexadecimal string</param>
        public RpcServerListener(string key = "", string iv = "")
        {
            this.Key = key;
            this.IV = iv;
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
        public async Task Accepting()
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
        public async Task Receiving(Socket socket)
        {
            using (var networkStream = new NetworkStream(socket, true))
            {
                // Initialize AES stream security
                Stream stream = networkStream; 
                if (!string.IsNullOrEmpty(this.Key) && !string.IsNullOrEmpty(this.IV))
                {
                    var cipher = new AesCryptoServiceProvider();
                    ByteEncoding.Hex.ToBytes(this.Key, cipher.Key, true);
                    ByteEncoding.Hex.ToBytes(this.IV, cipher.IV, true);
                    var transformer = cipher.CreateDecryptor(cipher.Key, cipher.IV);
                    stream = new CryptoStream(stream, transformer, CryptoStreamMode.Read);
                }

                // Attach JSON-RPC wrapper
                var rpc = JsonRpc.Attach(stream, this);
                await rpc.Completion;
            }
        }
    }
}
