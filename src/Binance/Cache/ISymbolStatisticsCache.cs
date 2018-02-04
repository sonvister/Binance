using System;
using System.Collections.Generic;
using Binance.Cache.Events;
using Binance.Market;
using Binance.WebSocket;

namespace Binance.Cache
{
    public interface ISymbolStatisticsCache
    {
        #region Events

        /// <summary>
        /// SymbolStatistics cache update event.
        /// </summary>
        event EventHandler<SymbolStatisticsCacheEventArgs> Update;

        #endregion Events

        #region  Properties

        /// <summary>
        /// The symbol statistics. Can be empty if not yet synchronized.
        /// </summary>
        IEnumerable<SymbolStatistics> Statistics { get; }

        /// <summary>
        /// The client that provides symbol statistics synchronization.
        /// </summary>
        ISymbolStatisticsWebSocketClient Client { get; }

        #endregion Properties

        #region Methods

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

        /// <summary>
        /// Unsubscribe from the currently subscribed symbol or symbols.
        /// </summary>
        void Unsubscribe();

        /// <summary>
        /// Link to a subscribed <see cref="ISymbolStatisticsWebSocketClient"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void LinkTo(ISymbolStatisticsWebSocketClient client, Action<SymbolStatisticsCacheEventArgs> callback = null);

        /// <summary>
        /// Unlink from client.
        /// </summary>
        void UnLink();

        #endregion Methods
    }
}
