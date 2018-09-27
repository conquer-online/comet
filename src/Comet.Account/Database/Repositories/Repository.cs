namespace Comet.Account.Database.Repositories
{
    using System;

    /// <summary>
    /// Implements generic data access layer (DAL) functions for Entity Framework 
    /// repositories. Keeps track of the database context instance for reusability 
    /// across database calls. Lifetime of the class instance should not exceed
    /// the duration of a transaction set. This is due to the lifetime and memory
    /// bloating of the database context from the Entity Framework.
    /// </summary>
    /// <typeparam name="TModel">
    /// Database model type from <see cref="Comet.Account.Database.Models" />
    /// </typeparam>
    public class Repository<TModel> : IDisposable
    {
        // Fields and Properties
        public readonly ServerDbContext Context;

        /// <summary>
        /// 
        /// </summary>
        public Repository()
        {
            this.Context = new ServerDbContext();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources. Implements the IDisposable design pattern
        /// for freeing database context resources.
        /// </summary>
        /// <param name="disposing">True to dispose from this instance</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
