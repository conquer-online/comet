namespace Comet.Network.RPC
{
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using StreamJsonRpc;

    /// <summary>
    /// Creates a TCP listener to listen for multiple remote procedure call requests on.
    /// RPC is implemented as JSON-RPC over TCP (the web socket / network stream). Once
    /// connected, clients will continue operating and sending requests for the life of
    /// the TCP connection or until the client is closed. The RPC network stream should
    /// never be exposed to the naked internet (use security groups or virtual networks
    /// if splitting between two VMs).
    /// </summary>
    public class RpcServerListener
    {
        // Fields and Properties
        protected TcpListener BaseListener;
        protected CancellationTokenSource ShutdownToken;
        private IRpcServerTarget Target;

        /// <summary>
        /// Instantiates a new instance of <see cref="RpcServerListener"/> using a
        /// target class of remote procedures.
        /// </summary>
        /// <param name="target">Target class for defining the RPC interface</param>
        public RpcServerListener(IRpcServerTarget target)
        {
            this.Target = target;
        }

        /// <summary>
        /// Binds the TCP listener to a port and network interface. Specify "0.0.0.0" 
        /// as the endpoint address to bind to all interfaces on the host machine. Starts
        /// the server listener and accepts new connections in a new task.
        /// </summary>
        /// <param name="port">Port number the server will bind to</param>
        /// <param name="address">Interface IPv4 address the server will bind to</param>
        /// <returns>Returns a new task for accepting new connections.</returns>
        public Task StartAsync(int port, string address = "0.0.0.0")
        {
            this.ShutdownToken = new CancellationTokenSource();
            this.BaseListener = new TcpListener(IPAddress.Parse(address), port);
            this.BaseListener.Start();
            return this.AcceptingAsync();
        }

        /// <summary>
        /// Accepting accepts client connections asynchronously as a new task. As a client
        /// connection is accepted, it will be associated with a new JSON-RPC wrapper. The 
        /// server will start processing requests from the client immediately after.
        /// </summary>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task AcceptingAsync()
        {
            while (this.BaseListener.Server.IsBound && !this.ShutdownToken.IsCancellationRequested)
            {
                var socket = await this.BaseListener.AcceptSocketAsync();
                var task = Task.Run(() => this.ReceivingAsync(socket));
            }
        }

        /// <summary>
        /// Receives commands from the RPC client connection.
        /// </summary>
        /// <param name="socket">Accepted client socket</param>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task ReceivingAsync(Socket socket)
        {            
            // Initialize streams
            using var stream = new NetworkStream(socket, true);
            Stream input = new BufferedStream(stream); 
            Stream output = new BufferedStream(stream);

            // Attach JSON-RPC wrapper
            var rpc = JsonRpc.Attach(output, input, this.Target);
            await rpc.Completion;
        }
    }
}
