using System;
using System.Collections.Generic;
using Binance.Api.WebSocket;
using Binance.Cache.Events;
using Binance.Market;

namespace Binance.Cache
{
    public interface IAggregateTradeCache
    {
        #region Public Events

        /// <summary>
        /// Aggregate trades update event.
        /// </summary>
        event EventHandler<AggregateTradeCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// The latest trades. Can be empty if not yet synchronized or out-of-sync.
        /// </summary>
        IEnumerable<AggregateTrade> Trades { get; }

        /// <summary>
        /// The client that provides trade information.
        /// </summary>
        IAggregateTradeWebSocketClient Client { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="callback"></param>
        void Subscribe(string symbol, int limit, Action<AggregateTradeCacheEventArgs> callback);

        /// <summary>
        /// Link to a subscribed <see cref="IAggregateTradeWebSocketClient"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        void LinkTo(IAggregateTradeWebSocketClient client, Action<AggregateTradeCacheEventArgs> callback = null);

        /// <summary>
        /// Unlink from client.
        /// </summary>
        void UnLink();

        #endregion Public Methods
    }
}
