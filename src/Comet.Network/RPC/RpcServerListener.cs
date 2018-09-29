namespace Comet.Network.RPC 
{
    using System.IO;
    using System.IO.Pipes;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using StreamJsonRpc;

    /// <summary>
    /// Creates a named pipe to listen for remote procedure call requests on. RPC is 
    /// implemented using a single full-duplex pipe stream and a JSON-RPC protocol wrapper.
    /// Once connected, clients will continue operating until the client is closed or the
    /// pipe connection is terminated.
    /// </summary>
    public abstract class RpcServerListener
    {
        // Fields and Properties
        protected JsonRpc Rpc;
        private NamedPipeServerStream BaseStream;

        /// <summary>
        /// Instantiates a new instance of <see cref="RpcServerListener"/> by creating a
        /// named pipe server stream. Does not start the listener until Start is called.
        /// </summary>
        /// <param name="pipeName">Name of the pipe</param>
        public RpcServerListener(string pipeName)
        {
            this.BaseStream = new NamedPipeServerStream(pipeName);
        }

        /// <summary>
        /// Starts listening for a single RPC client connection. Once connected and if
        /// security is enabled, the stream will be encrypted. The JSON-RPC server wrapper
        /// will then be attached to the final stream.
        /// </summary>
        /// <returns>Returns the running listener task.</returns>
        public async Task Start()
        {
            await this.BaseStream.WaitForConnectionAsync();
            using (var encryptor = new RSACryptoServiceProvider())
            using (var reader = new StreamReader(this.BaseStream))
            {
                
            }
        }
    }
}