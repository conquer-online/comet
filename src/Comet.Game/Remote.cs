namespace Comet.Game
{
    using System;
    using System.Runtime.Caching;
    using System.Security.Cryptography;
    using Comet.Network.RPC;
    using Comet.Shared.Models;

    /// <summary>
    /// Remote procedures that can be called from the account server to perform a game 
    /// server action. Methods in this class are automatically registered with the RPC
    /// server on server load.
    /// </summary>
    public class Remote : IRpcServerTarget
    {
        /// <summary>
        /// Called to ensure that the connection to the RPC server has been successful,
        /// and that any wire security placed on the connection has been initialized and
        /// is working correctly.
        /// </summary>
        /// <param name="agentName">Name of the client connecting</param>
        public void Connected(string agentName)
        {
            Console.WriteLine("{0} has connected", agentName);
        }

        /// <summary>
        /// Transfers authentication information directly from the account server on
        /// successful client login. The game server will generate a new access token,
        /// and return the token to the account server for client redirection. 
        /// </summary>
        /// <param name="args">Authentication details from the account server.</param>
        /// <returns>Returns an access token for the game server.</returns>
        public ulong TransferAuth(TransferAuthArgs args)
        {
            // Generate the access token
            var bytes = new byte[8];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            var token = BitConverter.ToUInt64(bytes);

            // Store in the login cache with an absolute timeout
            var timeoutPolicy = new CacheItemPolicy();
            timeoutPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(10);
            Kernel.Logins.Set(token.ToString(), args, timeoutPolicy) ;
            return token;
        }
    }
}
