using Binance.Api.WebSocket;
using Binance.Cache.Events;
using Binance.Market;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Cache
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
        /// The latest trades. Can be empty if not yet synchronized or out-of-sync.
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

        /// <summary>
        /// Link to a subscribed <see cref="ITradesWebSocketClient"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        /// <param name="leaveClientOpen"></param>
        void LinkTo(ITradesWebSocketClient client, Action<AggregateTradesCacheEventArgs> callback = null, bool leaveClientOpen = true);

        #endregion Public Methods
    }
}
