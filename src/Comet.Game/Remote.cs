namespace Comet.Game
{
    /// <summary>
    /// Remote procedures that can be called from the account server to perform a game 
    /// server action. Methods in this class are automatically registered with the RPC
    /// server on server load.
    /// </summary>
    public class Remote
    {
        public ulong TransferAuth(string ip, uint identity)
        {
            return 0;
        }
    }
}
