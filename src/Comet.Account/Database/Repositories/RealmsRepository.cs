namespace Comet.Account.Database.Repositories 
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Comet.Account.Database;
    using Comet.Account.Database.Models;
    using Comet.Network.RPC;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Repository for defining data access layer (DAL) logic for the realm table. Realm
    /// connection details are loaded into server memory at server startup, and may be 
    /// modified once loaded.
    /// </summary>
    public static class RealmsRepository
    {
        /// <summary>
        /// Loads realm connection details and security information to the server's pool
        /// of known realm routes. Should be invoked at server startup before the server
        /// listener has been started.
        /// </summary>
        public static async Task LoadAsync()
        {
            // Load realm connection information
            using var db = new ServerDbContext();
            Kernel.Realms = await db.Realms
                .Include(x => x.Authority)
                .ToDictionaryAsync(x => x.Name);

            // Connect to each realm's RPC server
            foreach (var realm in Kernel.Realms.Values)
            {
                realm.Rpc = new RpcClient();
                var task = realm.Rpc.ConnectAsync(
                    realm.RpcIPAddress, (int)realm.RpcPort, "Account Server");
            }
        }
    }
}
