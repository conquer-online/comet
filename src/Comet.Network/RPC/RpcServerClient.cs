namespace Comet.Network.RPC
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Threading;
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
    public class RpcClient
    {
        // Fields and Properties
        protected TcpClient BaseClient;
        protected JsonRpc Rpc;
        private byte[] Key, IV;

        /// <summary>
        /// Instantiates a new instance of <see cref="RpcClient"/> using an optional AES key
        /// and IV for JSON-RPC stream security. 
        /// </summary>
        /// <param name="key">AES key as a hexadecimal string</param>
        /// <param name="iv">AES IV as a hexadecimal string</param>
        public RpcClient(string key = "", string iv = "")
        {
            this.Key = new byte[key.Length / 2];
            this.IV = new byte[iv.Length / 2];
            ByteEncoding.Hex.ToBytes(key, this.Key);
            ByteEncoding.Hex.ToBytes(iv, this.IV);
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
            while (true)
            {
                try
                {
                    this.BaseClient = new TcpClient();
                    await this.BaseClient.ConnectAsync(address, port);
                    using (var stream = new NetworkStream(this.BaseClient.Client, true))
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
                        this.Rpc = JsonRpc.Attach(output, input);
                        await this.Rpc.InvokeAsync("Connected", null);
                        await this.Rpc.Completion;
                    }
                }
                catch (SocketException) { }
                catch (Exception e) { Console.WriteLine(e); }
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Returns true if the RPC server is online and the client is connected.
        /// </summary>
        public bool Online => this.BaseClient.Connected;

        /// <summary>
        /// Invoke a method on the server and do not wait for a result.
        /// </summary>
        /// <param name="method">Name of the remote procedure method</param>
        /// <param name="arg">Argument to pass with the request</param>
        /// <typeparam name="T">Type of response returned by the procedure</typeparam>
        /// <returns>Returns a task of the running RPC invoke.</returns>
        public async Task<T> Call<T>(string method, object arg)
        {
            return await this.Rpc.InvokeAsync<T>(method, arg);
        }

        /// <summary>
        /// Invoke a method on the server and do not wait for a result.
        /// </summary>
        /// <param name="method">Name of the remote procedure method</param>
        /// <param name="args">Arguments to pass with the request</param>
        /// <typeparam name="T">Type of response returned by the procedure</typeparam>
        /// <returns>Returns a task of the running RPC invoke.</returns>
        public async Task<T> Call<T>(string method, params object[] args)
        {
            return await this.Rpc.InvokeAsync<T>(method, args);
        }
    }
}
