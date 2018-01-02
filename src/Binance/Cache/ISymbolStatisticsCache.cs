using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket;
using Binance.Cache.Events;
using Binance.Market;

namespace Binance.Cache
{
    public interface ISymbolStatisticsCache
    {
        #region Public Events

        /// <summary>
        /// SymbolStatistics cache update event.
        /// </summary>
        event EventHandler<SymbolStatisticsCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// The symbol statistics. Can be empty if not yet synchronized.
        /// </summary>
        IEnumerable<SymbolStatistics> Statistics { get; }

        /// <summary>
        /// The client that provides symbol statistics synchronization.
        /// </summary>
        ISymbolStatisticsWebSocketClient Client { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Get statistics for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        SymbolStatistics GetStatistics(string symbol);

        /// <summary>
        /// Subscribe to all symbols.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync(Action<SymbolStatisticsCacheEventArgs> callback, CancellationToken token);

        /// <summary>
        /// Subscribe to a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync(string symbol, Action<SymbolStatisticsCacheEventArgs> callback, CancellationToken token);

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

        #endregion Public Methods
    }
}
