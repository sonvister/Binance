using System;
using System.Collections.Generic;
using Binance.Cache.Events;
using Binance.Market;
using Binance.WebSocket;

namespace Binance.Cache
{
    public interface ICandlestickCache
    {
        #region Events

        /// <summary>
        /// Candlesticks update event.
        /// </summary>
        event EventHandler<CandlestickCacheEventArgs> Update;

        #endregion Events

        #region Properties

        /// <summary>
        /// The candlesticks. Can be empty if not yet synchronized or out-of-sync.
        /// </summary>
        IEnumerable<Candlestick> Candlesticks { get; }

        /// <summary>
        /// The client that provides candlestick information.
        /// </summary>
        ICandlestickWebSocketClient Client { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="callback"></param>
        void Subscribe(string symbol, CandlestickInterval interval, int limit, Action<CandlestickCacheEventArgs> callback);

        /// <summary>
        /// Link to a subscribed <see cref="ICandlestickWebSocketClient"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        void LinkTo(ICandlestickWebSocketClient client, Action<CandlestickCacheEventArgs> callback = null);

        /// <summary>
        /// Unlink from client.
        /// </summary>
        void UnLink();

        #endregion Methods
    }
}
