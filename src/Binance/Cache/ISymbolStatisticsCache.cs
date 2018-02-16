using System;
using System.Collections.Generic;
using Binance.Cache.Events;
using Binance.Client;
using Binance.Market;

namespace Binance.Cache
{
    public interface ISymbolStatisticsCache : IJsonClientCache<ISymbolStatisticsClient, SymbolStatisticsCacheEventArgs>
    {
        /// <summary>
        /// The symbol statistics. Can be empty if not yet synchronized.
        /// </summary>
        IEnumerable<SymbolStatistics> Statistics { get; }

        /// <summary>
        /// Get statistics for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns><see cref="SymbolStatistics"/> or null if not subscribed to symbol or cache is not initialized.</returns>
        SymbolStatistics GetStatistics(string symbol);

        /// <summary>
        /// Get statistics for multiple symbols.
        /// </summary>
        /// <param name="symbols"></param>
        /// <returns><see cref="SymbolStatistics"/> or null if not subscribed to a symbol or cache is not initialized.</returns>
        IEnumerable<SymbolStatistics> GetStatistics(params string[] symbols);

        /// <summary>
        /// Subscribe to all symbols.
        /// </summary>
        /// <param name="callback"></param>
        void Subscribe(Action<SymbolStatisticsCacheEventArgs> callback);

        /// <summary>
        /// Subscribe to one or more symbols.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="symbols"></param>
        void Subscribe(Action<SymbolStatisticsCacheEventArgs> callback, params string[] symbols);
    }
}
