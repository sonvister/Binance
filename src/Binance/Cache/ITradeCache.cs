using System;
using System.Collections.Generic;
using Binance.Cache.Events;
using Binance.Market;
using Binance.WebSocket;

namespace Binance.Cache
{
    public interface ITradeCache
    {
        #region Public Events

        /// <summary>
        /// Trade update event.
        /// </summary>
        event EventHandler<TradeCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// The latest trades. Can be empty if not yet synchronized or out-of-sync.
        /// </summary>
        IEnumerable<Trade> Trades { get; }

        /// <summary>
        /// The client that provides trade information.
        /// </summary>
        ITradeWebSocketClient Client { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="callback"></param>
        void Subscribe(string symbol, int limit, Action<TradeCacheEventArgs> callback);

        /// <summary>
        /// Link to a subscribed <see cref="ITradeWebSocketClient"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        void LinkTo(ITradeWebSocketClient client, Action<TradeCacheEventArgs> callback = null);

        /// <summary>
        /// Unlink from client.
        /// </summary>
        void UnLink();

        #endregion Public Methods
    }
}
