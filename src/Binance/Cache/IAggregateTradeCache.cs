using System;
using System.Collections.Generic;
using Binance.Client;

namespace Binance.Cache
{
    public interface IAggregateTradeCache : IAggregateTradeCache<IAggregateTradeClient>
    { }

    public interface IAggregateTradeCache<TClient> : IJsonClientCache<TClient, AggregateTradeCacheEventArgs>
        where TClient : IAggregateTradeClient
    {
        /// <summary>
        /// Aggregate trades out-of-sync event.
        /// </summary>
        event EventHandler<EventArgs> OutOfSync;

        /// <summary>
        /// The latest trades. Can be empty if not yet synchronized or out-of-sync.
        /// </summary>
        IEnumerable<AggregateTrade> Trades { get; }

        /// <summary>
        /// Subscribe to a symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="limit">The number of trades to cache.</param>
        /// <param name="callback">The callback (optional).</param>
        void Subscribe(string symbol, int limit, Action<AggregateTradeCacheEventArgs> callback);
    }
}
