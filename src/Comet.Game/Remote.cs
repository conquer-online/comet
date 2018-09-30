namespace Comet.Game
{
    using System;

    /// <summary>
    /// Remote procedures that can be called from the account server to perform a game 
    /// server action. Methods in this class are automatically registered with the RPC
    /// server on server load.
    /// </summary>
    public class Remote
    {
        public void Connected(string name)
        {
            Console.WriteLine("{0} has connected", name);
        }

        public ulong TransferAuth(string ip, uint identity)
        {
            return 0;
        }
    }
}
