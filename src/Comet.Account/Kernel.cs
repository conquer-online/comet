namespace Comet.Account
{
    using System.Collections.Generic;
    using Comet.Account.Database.Models;

    /// <summary>
    /// Kernel for the server, acting as a central core for pools of models and states
    /// initialized by the server. Used in database repositories to load data into memory
    /// from essential tables or tables which require heavy post-processing. Used in the
    /// server packet process methods for tracking client and world states.
    /// </summary>
    public static class Kernel
    {
        public static Dictionary<string, Realm> Realms;
    }
}