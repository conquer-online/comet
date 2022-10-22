namespace Comet.Network.RPC
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using StreamJsonRpc;

    /// <summary>
    /// Creates a TCP client to connect to an RPC server listener. RPC is implemented as
    /// JSON-RPC over TCP (a web socket / network stream). Once the client connects, it can
    /// remain connected for the life of the TCP connection or until the server closes.
    /// The client should never be exposed to the naked internet (use security groups or
    /// virtual networks if splitting between two VMs).
    /// </summary>
    public class RpcClient
    {
        // Fields and Properties
        protected TcpClient BaseClient;
        protected JsonRpc Rpc;

        /// <summary>
        /// Connects the client to a remote TCP host using the specified host name and port
        /// number. Once the client has connected, the stream will be wrapped in a JSON-RPC
        /// protocol wrapper.
        /// </summary>
        /// <param name="address">IP address of the RPC server</param>
        /// <param name="port">Port the RPC server is listening on</param>
        /// <param name="agent">The name of the client</param>
        /// <returns>Returns a new task for connecting and fault tolerance.</returns>
        public async Task ConnectAsync(string address, int port, string agent = "Client")
        {
            while (true)
            {
                try
                {
                    this.BaseClient = new TcpClient();
                    await this.BaseClient.ConnectAsync(address, port);
                    using var stream = new NetworkStream(this.BaseClient.Client, true);
                    
                    // Attach JSON-RPC wrapper
                    this.Rpc = JsonRpc.Attach(stream, stream);
                    await this.Rpc.InvokeAsync("Connected", agent);
                    await this.Rpc.Completion;
                }
                catch (IOException) { }
                catch (SocketException) { }
                catch (Exception e) { Console.WriteLine(e); }
                Thread.Sleep(1000);
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
        public Task<T> CallAsync<T>(string method, object arg)
        {
            return this.Rpc.InvokeAsync<T>(method, arg);
        }

        /// <summary>
        /// Invoke a method on the server and do not wait for a result.
        /// </summary>
        /// <param name="method">Name of the remote procedure method</param>
        /// <param name="args">Arguments to pass with the request</param>
        /// <typeparam name="T">Type of response returned by the procedure</typeparam>
        /// <returns>Returns a task of the running RPC invoke.</returns>
        public Task<T> CallAsync<T>(string method, params object[] args)
        {
            return this.Rpc.InvokeAsync<T>(method, args);
        }
    }
}
