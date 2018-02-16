using System;
using Binance.Cache.Events;
using Binance.Client;

namespace Binance.Cache
{
    /// <summary>
    /// An <see cref="IJsonClient"/> with in-memory data store.
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    /// <typeparam name="TEventArgs"></typeparam>
    public interface IJsonClientCache<TClient, TEventArgs> : IJsonClient
        where TClient : IJsonClient
        where TEventArgs : CacheEventArgs
    {
        /// <summary>
        /// The update event.
        /// </summary>
        event EventHandler<TEventArgs> Update;

        /// <summary>
        /// Get or set the JSON client.
        /// </summary>
        TClient Client { get; set; }
    }
}
