using Binance.Trades;
using Binance.Trades.Cache;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Binance
{
    public interface IAggregateTradesCache : IDisposable
    {
        #region Public Events

        /// <summary>
        /// Aggregate trades update event.
        /// </summary>
        event EventHandler<AggregateTradesCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// The latest trades.
        /// </summary>
        IEnumerable<AggregateTrade> Trades { get; }

        /// <summary>
        /// The client that provides trade information.
        /// </summary>
        ITradesWebSocketClient Client { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync(string symbol, int limit = default, CancellationToken token = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync(string symbol, Action<AggregateTradesCacheEventArgs> callback, int limit = default, CancellationToken token = default);

        #endregion Public Methods
    }
}
