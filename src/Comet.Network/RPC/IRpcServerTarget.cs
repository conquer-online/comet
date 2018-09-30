namespace Comet.Network.RPC
{
    /// <summary>
    /// A C# interface for defining an RPC target interface. Specifies a Connected procedure
    /// to ensure that the connection has been successful and that any sort of wire security
    /// has been initialized correctly.
    /// </summary>
    public interface IRpcServerTarget
    {
        void Connected(string agentName);
    }
}